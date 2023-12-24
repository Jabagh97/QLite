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
            if (!string.IsNullOrEmpty(searchValue))
            {
                // Assuming that all properties should be included in the search
                return data.Where($"ToString().Contains(@0)", searchValue);
            }

            return data;
        }

        public IQueryable<object> ApplySorting(IQueryable<object> data, string sortColumn, string sortColumnDirection, Type modelType)
        {
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                return data.OrderBy($"{sortColumn} {sortColumnDirection}");
            }

            return data;
        }


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IQueryable<object> data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            // Filter properties based on ViewModel
            var filteredData = FilterPropertiesBasedOnViewModel(data, modelType, viewModelType, modelTypeMapping);

            // Apply search filter
            filteredData = ApplySearchFilter(data, searchValue);

            // Apply sorting
            filteredData = ApplySorting(data, sortColumn, sortColumnDirection, modelType);

            return filteredData;
        }

        private IQueryable FilterPropertiesBasedOnViewModel(IQueryable<object> data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var modelProperties = modelType.GetProperties().Select(p => p.Name).ToList();
            var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();

            var commonProperties = modelProperties.Intersect(viewModelProperties).ToList();

            if (commonProperties.Any())
            {
                // Construct the dynamic select expression
                var selectExpression = string.Join(", ", commonProperties);

                // Project the data to include only common properties


                // Paginate the data
                var paginatedData = data.Select($"new ({selectExpression})")  ;
                  

                return paginatedData ;

               
            }
            else
            {
                return data;
            }
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
