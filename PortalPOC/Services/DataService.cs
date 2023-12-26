using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using PortalPOC.Models;

using System.Linq.Dynamic.Core;

using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


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
                var typedDbSet = setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);

                // Log or print the DbSet information for debugging
                Console.WriteLine($"Typed DbSet for {modelType.Name}: {typedDbSet}");

                return (IQueryable)typedDbSet;
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"Error getting DbSet for type {modelType.Name}: {ex.Message}");

                // Rethrow the exception for better diagnosis
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
            // data = ApplySearchFilter(data, searchValue, modelType);

            // Apply sorting
            // data = ApplySorting(data, sortColumn, sortColumnDirection, modelType);

            return data;
        }


        private IQueryable FilterPropertiesBasedOnViewModel(IQueryable data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var viewModelProperties = viewModelType.GetProperties().Select(p => p.Name).ToList();
            var modelProperties = modelType.GetProperties().Where(p => viewModelProperties.Contains(p.Name)).ToList();

            if (!modelProperties.Any())
            {
                return data;
            }

            IQueryable intermediateData = data;

            foreach (var propertyInfo in modelProperties.Where(p => Nullable.GetUnderlyingType(p.PropertyType) == typeof(Guid)))
            {
                var (relatedType, relatedKeyType) = modelTypeMapping[propertyInfo.Name];

                // Create key selector expressions
                var outerKeySelector = $"it.{propertyInfo.Name}";
                var innerKeySelector = "Oid";

                // Define a mapping for special cases
                string propertyToJoin;
                switch (propertyInfo.Name)
                {
                    case "KioskApplication":
                        propertyToJoin = "KappName";
                        break;
                    // Add more cases as needed
                    default:
                        propertyToJoin = "Name";
                        break;
                }

                // Build the dynamic select expression with braces for each iteration
                //var resultSelector = $"new ({string.Join(", ", modelProperties.Select(p => $"outer.{p.Name} as {p.Name}"))})";

                // Replace the specific part for the related property
                //resultSelector = resultSelector.Replace($"outer.{propertyInfo.Name} as {propertyInfo.Name}", $"inner.{propertyToJoin} as {propertyInfo.Name}");

                // Accumulate left join operations
                intermediateData = DynamicQueryableExtensions.GroupJoin(
         intermediateData,
         GetTypedDbSet(relatedType),
         outerKeySelector,
         innerKeySelector,
         $"new ({string.Join(", ", modelProperties.Select(p => $"outer.{p.Name} as {p.Name}"))}, inner as Test1  )"
     );

                // Select the desired properties from the left join result
              

                Console.WriteLine(intermediateData.ToQueryString());
                //break;
            }

            return intermediateData;
        }


        private object GetPropertyValue(object item, PropertyInfo property)
        {
            return property.GetValue(item);
        }

        #endregion
    }

}
