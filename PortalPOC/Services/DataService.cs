using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Collections;

namespace PortalPOC.Services
{
    public class DataService : IDataService
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

        public DataService(QuavisQorchAdminEasyTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<object> GetTypedDbSet(Type modelType)
        {
            var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
            return ((IEnumerable)setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null)).Cast<object>();
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
            var propertyInfo = modelType.GetProperty(sortColumn);

            if (propertyInfo != null)
            {
                data = sortColumnDirection == "asc"
                    ? data.OrderBy(item => propertyInfo.GetValue(item, null))
                    : data.OrderByDescending(item => propertyInfo.GetValue(item, null));
            }

            return data;
        }
    }

}
