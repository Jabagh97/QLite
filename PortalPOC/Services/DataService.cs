using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Collections;
using System.Dynamic;
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
        public IEnumerable<object> GetTypedDbSet(Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                return ((IEnumerable)setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null)).Cast<object>();
            }
            catch (Exception ex)
            {
                // Handle the exception (log or rethrow)
                throw new InvalidOperationException($"Error getting DbSet for type {modelType.Name}", ex);
            }
        }


        public IEnumerable<object> ApplySearchFilter(IEnumerable<object> data, string searchValue)
        {
            return data.AsEnumerable().Where(item =>
                item.GetType().GetProperties()
                    .Any(prop => prop.PropertyType == typeof(string) && prop.GetValue(item)?.ToString().Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true)
            );
        }

        public IEnumerable<object> ApplySorting(IEnumerable<object> data, string sortColumn, string sortColumnDirection, Type modelType)
        {
            if (data == null || string.IsNullOrEmpty(sortColumn) || modelType == null)
            {
                throw new ArgumentNullException("Invalid input parameters");
            }

            var propertyInfo = modelType.GetProperty(sortColumn);

            if (propertyInfo != null)
            {
                data = sortColumnDirection == "asc"
                    ? data.OrderBy(item => propertyInfo.GetValue(item, null))
                    : data.OrderByDescending(item => propertyInfo.GetValue(item, null));
            }

            return data;
        }


        public IEnumerable<object> GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IEnumerable<object> data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            // Apply search filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                data = ApplySearchFilter(data, searchValue);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                data = ApplySorting(data, sortColumn, sortColumnDirection, modelType);
            }

            // Filter properties based on ViewModel
            var filteredData = FilterPropertiesBasedOnViewModel(data, modelType, viewModelType, modelTypeMapping);

            return filteredData;
        }
        private IEnumerable<object> FilterPropertiesBasedOnViewModel(IEnumerable<object> data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            return data.Select(item => MapModelToViewModelProperties(item, modelType, viewModelType, modelTypeMapping));
        }

        private Dictionary<string, object> MapModelToViewModelProperties(object item, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
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
        }
        private object GetPropertyValue(object item, PropertyInfo property)
        {
            return property.GetValue(item);
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

                var relatedEntity = relatedDbSetInstance.Cast<dynamic>()
                    .Where(e => ((Guid)e.Oid) == (Guid)relatedValue)
                    .Select(e => new { e.Name }) // Only fetch necessary properties
                    .FirstOrDefault();

                var name = relatedEntity?.Name;

                filteredItem[propertyName] = name;
            }
        }
        #endregion
    }

}
