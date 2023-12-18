using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Collections;
using System.Dynamic;
using System.Security.Cryptography;

namespace PortalPOC.Services
{
    public class DataService : IDataService
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

        public DataService(QuavisQorchAdminEasyTestContext dbContext)
        {
            _dbContext = dbContext;
        }


        #region GetData
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
            var filteredData = data.Select(item =>
            {
                var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();
                var modelProperties = modelType.GetProperties().Where(p => viewModelProperties.Contains(p.Name)).ToList();

                var filteredItem = new Dictionary<string, object>();

                foreach (var property in modelProperties)
                {
                    var value = item.GetType().GetProperty(property.Name)?.GetValue(item);

                    // Handle Guid or Oid properties
                    if (value is Guid || value is Oid)
                    {
                        // Get the name from the related table
                        var relatedPropertyName = property.Name;
                        var relatedValue = item.GetType().GetProperty(relatedPropertyName)?.GetValue(item);

                        // Check if relatedValue is not null and is of type Oid or Guid
                        if (relatedValue != null && (relatedValue is Oid || relatedValue is Guid))
                        {
                            // Get the DbSet of the related property
                            if (modelTypeMapping.TryGetValue(property.Name, out var relatedDbSet))
                            {
                                var RelatedDbSet = GetTypedDbSet(relatedDbSet.Item1);

                                // Perform a LINQ query to join and retrieve the name
                                var relatedEntity = RelatedDbSet.Cast<dynamic>()
                                    .Where(e => ((Guid)e.Oid) == (Guid)relatedValue)
                                    .Select(e => new { e.Name }) // Only fetch necessary properties
                                    .FirstOrDefault();

                                var name = relatedEntity?.Name;

                                filteredItem[property.Name] = name;
                            }
                        }
                        else
                        {
                            filteredItem[property.Name] = relatedValue;
                        }
                    }
                    else
                    {
                        filteredItem[property.Name] = value;
                    }
                }

                return filteredItem;
            });

            return filteredData;
        }

        #endregion
    }

}
