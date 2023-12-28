using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using PortalPOC.Helpers;
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
        public IQueryable? GetTypedDbSet(Type modelType)
        {
            try
            {
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);
                var typedDbSet = setMethod!.MakeGenericMethod(modelType).Invoke(_dbContext, null);

                // Log or print the DbSet information for debugging
                Console.WriteLine($"Typed DbSet for {modelType.Name}: {typedDbSet}");

                return typedDbSet as IQueryable;
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"Error getting DbSet for type {modelType.Name}: {ex.Message}");

                // Rethrow the exception for better diagnosis
                throw new InvalidOperationException($"Error getting DbSet for type {modelType.Name}", ex);
            }
        }



        public IQueryable ApplySearchFilter(IQueryable data, string? searchValue, Type modelType, Type viewModelType)
        {
            if (!string.IsNullOrEmpty(searchValue))
            {
                var commonProperties = modelType.GetProperties()
                    .Where(m => m.PropertyType == typeof(string) && viewModelType.GetProperty(m.Name) != null)
                    .Select(m => m.Name);

                var filterExpression = string.Join(" OR ", commonProperties.Select(p => $"{p}.ToLower().Contains(@0)"));

                return data.Where(filterExpression, searchValue.ToLower());
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


        public IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            var dbSet = GetTypedDbSet(modelType);

            var data = dbSet?.Where("Gcrecord == null");

            data = FilterPropertiesBasedOnViewModel(data, modelType, viewModelType, modelTypeMapping);

      
            data = ApplySearchFilter(data, searchValue, modelType, viewModelType);

          
             data = ApplySorting(data, sortColumn, sortColumnDirection);

            return data;
        }



        private IQueryable FilterPropertiesBasedOnViewModel(IQueryable? data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            switch (modelType.Name)
            {
                case "Branch":
                    return QueriesHelper.BranchProperties(_dbContext);
                case "Country":
                    return QueriesHelper.CountryProperties(_dbContext);
                case "Province":
                    return QueriesHelper.ProvinceProperties(_dbContext);
                case "Resource":
                    return QueriesHelper.ResourceProperties(_dbContext);
                case "SubProvince":
                    return QueriesHelper.SubProvinceProperties(_dbContext);
                case "KappSetting":
                    return QueriesHelper.KappSettingProperties(_dbContext);
                case "KioskApplication":
                    return QueriesHelper.KioskApplicationProperties(_dbContext);
                case "KioskApplicationType":
                    return QueriesHelper.KioskApplicationTypeProperties(_dbContext);
                case "Language":
                    return QueriesHelper.LanguageProperties(_dbContext);
                default:
                    throw new NotSupportedException($"Model type '{modelType.Name}' is not supported.");
            }
        }

       



        #endregion
    }

}
