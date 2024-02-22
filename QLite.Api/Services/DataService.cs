using Microsoft.EntityFrameworkCore;
using QLiteDataApi.Constants;

using QLiteDataApi.QueryFactory;
using QLite.Data;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using QLiteDataApi.Helpers;
using QLiteDataApi.Context;

namespace QLiteDataApi.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly IQueryFactory _queryFactory;

        public DataService(ApplicationDbContext dbContext, IQueryFactory queryFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryFactory = queryFactory ?? throw new ArgumentNullException(nameof(queryFactory));

        }
        public IQueryable? GetTypedDbSet(Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                var typedDbSet = setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);


                return typedDbSet as IQueryable;
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"{Errors.ErrorGettingDbSet} {modelType.Name}: {ex.Message}");

                // Rethrow the exception for better diagnosis
                throw new InvalidOperationException($"{Errors.ErrorGettingDbSet} {modelType.Name}: {ex.Message}", ex);
            }
        }


        #region GetData



        public IQueryable ApplySearchFilter(IQueryable data, string? searchValue, Type modelType, Type viewModelType)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var commonProperties = modelType.GetProperties()
                    .Where(m => m.PropertyType == typeof(string) && viewModelType.GetProperty(m.Name) != null)
                    .Select(m => m.Name);

                var filterExpression = string.Join(" OR ", commonProperties.Select(p => $"{p}.ToLower().Contains(@0)"));

                return data.Where(filterExpression, searchValue);
            }

            return data;
        }


        public IQueryable ApplySorting(IQueryable data, string? sortColumn, string? sortColumnDirection)
        {
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                return data.OrderBy($"{sortColumn} {sortColumnDirection}");
            }

            return data;
        }


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection)
        {
            var dbSet = GetTypedDbSet(modelType);
            var data = dbSet;


            IQuery Model = _queryFactory.GetModel(modelType.Name);

            data = Model.SelectAndJoinQuery(data, modelType, viewModelType, _dbContext);


            data = ApplySearchFilter(data, searchValue, modelType, viewModelType);
            data = ApplySorting(data, sortColumn, sortColumnDirection);

            return data;
        }


        public IQueryable GetTabData(Type innerType, Type innerViewType, string Oid, Type mainModelType)
        {
            var dbSet = GetTypedDbSet(innerType);
            var data = dbSet;

            IQuery Model = _queryFactory.GetModel(innerType.Name);


            var optionalArguments = new Dictionary<string, object>
            {
                [mainModelType.Name] = Oid,
            };

            data = Model.SelectAndJoinQuery(data, innerType, innerViewType, _dbContext, optionalArguments);

            return data;
        }

        #endregion


        #region Create&EditData
        public Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var namesDictionary = new Dictionary<string, List<dynamic>>();

            var guidProperties = modelType.GetProperties().Where(p => p.PropertyType == typeof(Guid?));

            foreach (var guidProperty in guidProperties)
            {
                if (string.IsNullOrEmpty(guidProperty.Name) || !modelTypeMapping.TryGetValue(guidProperty.Name, out var relatedTypeTuple))
                {
                    continue; // Skip to the next property in case of an error
                }

                var names = GetRelatedEntityNames(relatedTypeTuple.Item1);

                namesDictionary[guidProperty.Name] = names;
            }

            return namesDictionary;
        }

        private List<dynamic> GetRelatedEntityNames(Type relatedEntityType)
        {
            var propertyName = relatedEntityType.Name switch
            {
                Properties.KioskApplication => Properties.KappName,
                _ => Properties.Name
            };

            var relatedEntities = GetTypedDbSet(relatedEntityType);
            return relatedEntities?.Where($"{Properties.Gcrecord} == null").Select($"new ({propertyName} as {propertyName}, {Properties.Oid} as {Properties.Oid})").ToDynamicList()
                ?? new List<dynamic>();
        }

        public object CreateModel(string user, Type modelType, Dictionary<string, object> formData)
        {
            // Validate and create a model instance
            var modelInstance = Activator.CreateInstance(modelType);

            if (modelInstance == null)
            {
                throw new InvalidOperationException(Errors.NotCreated);
            }

            foreach (var property in formData)
            {
                var propertyInfo = modelType.GetProperty(property.Key);

                if (propertyInfo != null)
                {
                    // Convert the value to the property type
                    var convertedValue = QueriesHelper.ConvertToType(property.Value?.ToString(), propertyInfo.PropertyType);

                    // Set the property value
                    propertyInfo.SetValue(modelInstance, convertedValue);
                }
            }

            IQuery Model = _queryFactory.GetModel(modelType.Name);

            // Save the created model instance to your data store or perform any necessary operations
            Model.CreateInstance(user, _dbContext, modelInstance!);

            // Return the created model instance
            return modelInstance;
        }

        public object UpdateModel(string user, Type modelType, Dictionary<string, object> formData)
        {
            // Validate and update the model instance
            if (!formData.ContainsKey(Properties.Oid))
            {
                throw new ArgumentException(Errors.NullOid);
            }

            var idValue = formData[Properties.Oid].ToString();

            var dbSet = GetTypedDbSet(modelType);

            IQuery Model = _queryFactory.GetModel(modelType.Name);

            // Fetch the existing entity from your data store
            var existingEntity = Model.GetById(idValue, dbSet);

            if (existingEntity == null)
            {
                throw new ArgumentException(Errors.EntityNotFound);
            }

            foreach (var property in formData)
            {
                var propertyName = property.Key;
                var propertyInfo = modelType.GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    // Convert the value to the property type
                    var convertedValue = QueriesHelper.ConvertToType(property.Value?.ToString(), propertyInfo.PropertyType);

                    // Set the property value
                    propertyInfo.SetValue(existingEntity, convertedValue);
                }
            }

            // Save changes to your data store or perform any necessary operations
            Model.UpdateInstance(user, _dbContext, existingEntity);

            // Return the updated model instance
            return existingEntity;
        }


        #endregion


        #region SoftDelete

        public bool SoftDelete(Type modelType, Dictionary<string, object> formData)
        {
            try
            {
                // Ensure the dictionary contains the primary key value
                if (!formData.TryGetValue(Properties.Oid, out var oidJsonElement) || oidJsonElement == null)
                {
                    throw new ArgumentException(Errors.NullOid);
                }

                // Extract primary key values
                var primaryKeyValues = JsonSerializer.Deserialize<List<string>>(oidJsonElement.ToString());

                // Get the DbSet for the specified model type
                var dbSet = GetTypedDbSet(modelType);

                foreach (var primaryKeyValue in primaryKeyValues)
                {
                    // Trim and replace as needed
                    var cleanedPrimaryKeyValue = primaryKeyValue?.Trim('[', ']').Replace("\"", "");

                    if (!string.IsNullOrEmpty(cleanedPrimaryKeyValue))
                    {
                        IQuery Model = _queryFactory.GetModel(modelType.Name);

                        // Find the entity by its primary key
                        var entity = Model.SoftDeleteInstance(_dbContext, dbSet, modelType, cleanedPrimaryKeyValue);

                    }
                }

                return true; // Soft delete successful
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return false; // Soft delete failed
            }
        }

        public bool RemoveFromSubList(string tabName, Type modelType, string modelOid, List<string> Oids)
        {
            IQuery Model = _queryFactory.GetModel(modelType.Name);

            bool allSuccess = true;

            foreach (var oid in Oids)
            {
                var result = Model.RemoveFromSubList(_dbContext, tabName, modelType, modelOid, oid);

                // Update the 'allSuccess' flag based on the result of each iteration
                allSuccess = allSuccess && result;
            }

            return allSuccess;
        }




        #endregion

    }

}
