using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;

using System.Linq.Dynamic.Core;

using System.Reflection;


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
        public IQueryable GetTypedDbSet(Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                return (IQueryable)setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);
            }
            catch (Exception ex)
            {
                // Handle the exception (log or rethrow)
                throw new InvalidOperationException($"Error getting DbSet for type {modelType.Name}", ex);
            }
        }


        public IQueryable ApplySearchFilter(IQueryable data, string searchValue, Type modelType)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var properties = modelType.GetProperties()
                                           .Where(p => p.PropertyType == typeof(string))
                                           .Select(p => p.Name);

                var filterExpression = string.Join(" OR ", properties.Select(p => $"{p}.Contains(@0)"));

                return data.Where(filterExpression, searchValue);
            }

            return data;
        }


        public IQueryable ApplySorting(IQueryable data, string sortColumn, string sortColumnDirection, Type modelType)
        {
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                return data.OrderBy($"{sortColumn} {sortColumnDirection}");
            }

            return data;
        }


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IQueryable data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            // Only Select View Properties from Model

            data = FilterPropertiesBasedOnViewModel(data, modelType, viewModelType, modelTypeMapping);

            // Apply search filter
            data = ApplySearchFilter(data, searchValue, modelType);

            // Apply sorting
            data = ApplySorting(data, sortColumn, sortColumnDirection, modelType);

            return data;
        }

        private IQueryable FilterPropertiesBasedOnViewModel(IQueryable data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();
            var modelProperties = modelType.GetProperties().Where(p => viewModelProperties.Contains(p.Name)).ToList();

            if (modelProperties.Any())
            {
                // Construct the dynamic select expression with braces
                var selectExpression = GenerateSelectExpression(modelProperties, modelType, modelTypeMapping);

                // Project the data to include only common properties 
                return data.Select($"new ({selectExpression})");
            }
            else
            {
                return data;
            }
        }

        private string GenerateSelectExpression(List<PropertyInfo> modelProperties, Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var selectExpressions = new List<string>();

            // Start with the main entity (e.g., "branch")
            selectExpressions.Add($"{modelType.Name}.*");

            foreach (var property in modelProperties)
            {
                var propertyName = property.Name;

                // Handle Guid or Oid properties
                if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Guid))
                {
                    var relatedEntityName = GetRelatedEntityName(propertyName, property.PropertyType, modelTypeMapping);
                    var relatedPropertyName = "Name";

                    // Join operation for Guid properties
                    var joinExpression = $"left join {relatedEntityName} on {relatedEntityName}.Oid = {modelType.Name}.{propertyName}";
                    var valueExpression = $"{relatedEntityName}.{relatedPropertyName} as {propertyName}";

                    selectExpressions.Add(joinExpression + " " + valueExpression);
                }
            }

            return string.Join(", ", selectExpressions);
        }




        private string GetRelatedEntityName(string propertyName, Type propertyType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            if (propertyType == typeof(Guid) && modelTypeMapping.TryGetValue(propertyName, out var relatedDbSet))
            {
                return relatedDbSet.Item2.Name;
            }

            return propertyName;
        }






        private object GetPropertyValue(object item, PropertyInfo property)
        {
            return property.GetValue(item);
        }
        private void ProcessGuidOrOidProperty(object value, string propertyName, object item, Dictionary<string, (Type, Type)> modelTypeMapping, Dictionary<string, object> filteredItem)
        {

        }

        private void ProcessRelatedGuidOrOidProperty(object relatedValue, string propertyName, Dictionary<string, (Type, Type)> modelTypeMapping, Dictionary<string, object> filteredItem)
        {

        }
        #endregion
    }

}
