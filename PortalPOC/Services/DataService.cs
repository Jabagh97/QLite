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
           

            // Apply search filter
            data = ApplySearchFilter(data, searchValue, modelType);

            // Apply sorting
            data = ApplySorting(data, sortColumn, sortColumnDirection, modelType);

            return data;
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
