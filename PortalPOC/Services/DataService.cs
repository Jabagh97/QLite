using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace PortalPOC.Services
{
    public class DataService : IDataService
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

        public DataService(QuavisQorchAdminEasyTestContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        #region GetData
        public IQueryable<object> GetTypedDbSet(Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                return (IQueryable<object>)setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);
            }
            catch (Exception ex)
            {
                // Handle the exception (log or rethrow)
                throw new InvalidOperationException($"Error getting DbSet for type {modelType.Name}", ex);
            }
        }


        public IQueryable<object> ApplySearchFilter(IQueryable<object> data, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return data;
            }

            return data.Where(item =>
                item.GetType().GetProperties()
                    .Any(prop => prop.PropertyType == typeof(string) &&
                                  EF.Property<string>(item, prop.Name).Contains(searchValue, StringComparison.OrdinalIgnoreCase))
            );
        }


        public IQueryable<object> ApplySorting(IQueryable<object> data, string sortColumn, string sortColumnDirection, Type modelType)
        {
            if (data == null || string.IsNullOrEmpty(sortColumn) || modelType == null)
            {
                throw new ArgumentNullException("Invalid input parameters");
            }

            var parameter = Expression.Parameter(modelType, "item");
            var property = Expression.Property(parameter, sortColumn);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = sortColumnDirection == "asc" ? "OrderBy" : "OrderByDescending";

            // Use reflection to get the generic method
            var orderByMethod = typeof(Queryable).GetMethods()
                .Where(method => method.Name == methodName && method.IsGenericMethodDefinition)
                .Single(method => method.GetParameters().Length == 2)
                .MakeGenericMethod(modelType, property.Type);

            var methodCallExpression = Expression.Call(
                null,
                orderByMethod,
                Expression.Convert(data.Expression, typeof(IQueryable<>).MakeGenericType(modelType)), // Cast to correct type
                Expression.Quote(lambda)
            );

            return data.Provider.CreateQuery<object>(methodCallExpression);
        }

        public IQueryable<object> GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IQueryable<object> data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var parameter = Expression.Parameter(modelType, "item");
            var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();
            var modelProperties = modelType.GetProperties().Where(p => viewModelProperties.Contains(p.Name)).ToList();

            // Get the list of property names from modelProperties
            var propertyNames = modelProperties.Select(p => p.Name).ToArray();

            // Use the Select extension method with the property names
            return SelectCustom(data, propertyNames, modelType);
        }


        public static IQueryable<object> SelectCustom(IQueryable source, string[] columns, Type modelType)
        {
            var sourceType = source.ElementType;
            var resultType = modelType.GetType();
            var parameter = Expression.Parameter(sourceType, "e");
            var bindings = columns.Select(column => Expression.Bind(
                resultType.GetProperty(column), Expression.PropertyOrField(parameter, column)));
            var body = Expression.MemberInit(Expression.New(resultType), bindings);
            var selector = Expression.Lambda(body, parameter);
            return source.Provider.CreateQuery<object>(
                Expression.Call(typeof(Queryable), "Select", new Type[] { sourceType, resultType },
                    source.Expression, Expression.Quote(selector)));
        }


        private IEnumerable<object> FilterPropertiesBasedOnViewModel(IEnumerable<object> data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            return data.Select(item => {

                var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();
                var modelProperties = modelType.GetProperties().Where(p => viewModelProperties.Contains(p.Name)).ToList();

                var filteredItem = new Dictionary<string, object>();

                foreach (var property in modelProperties)
                {
                    var value = GetPropertyValue(item, property);

                    // Handle Guid or Oid properties
                    if (value is Guid || value is Oid)
                    {
                        // Process Guid or Oid properties
                        ProcessGuidOrOidProperty(value, property.Name, item, modelTypeMapping, filteredItem);
                    }
                    else
                    {
                        filteredItem[property.Name] = value;
                    }
                }

                return filteredItem;
            });
        }

        private void ProcessGuidOrOidProperty(object value, string propertyName, object item, Dictionary<string, (Type, Type)> modelTypeMapping, Dictionary<string, object> filteredItem)
        {
            var relatedPropertyName = propertyName;
            var relatedValue = GetPropertyValue(item, item.GetType().GetProperty(relatedPropertyName));

            if (relatedValue is null)
            {
                filteredItem[propertyName] = null;
            }
            else if (relatedValue is Oid || relatedValue is Guid)
            {
                // Process related Oid or Guid properties
                ProcessRelatedGuidOrOidProperty(relatedValue, propertyName, modelTypeMapping, filteredItem);
            }
            else
            {
                filteredItem[propertyName] = relatedValue;
            }
        }

        private void ProcessRelatedGuidOrOidProperty(object relatedValue, string propertyName, Dictionary<string, (Type, Type)> modelTypeMapping, Dictionary<string, object> filteredItem)
        {
            if (modelTypeMapping.TryGetValue(propertyName, out var relatedDbSet))
            {
                var relatedDbSetInstance = GetTypedDbSet(relatedDbSet.Item1);

                // In-memory processing, not translated to SQL
                var relatedEntity = relatedDbSetInstance
                    .AsEnumerable()  // Switch to in-memory processing
                    .Where(e => EF.Property<Guid>(e, "Oid") == (Guid)relatedValue)
                    .Select(e => EF.Property<string>(e, "Name"))
                    .FirstOrDefault(); // Only fetch necessary properties

                filteredItem[propertyName] = relatedEntity;
            }
        }



        private object GetPropertyValue(object item, PropertyInfo property)
        {
            return property.GetValue(item);
        }

  
        #endregion
    }

}
