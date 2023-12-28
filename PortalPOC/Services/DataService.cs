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



        public IQueryable ApplySearchFilter(IQueryable data, string searchValue, Type modelType, Type viewModelType)
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
            data = ApplySearchFilter(data, searchValue, modelType, viewModelType);

            // Apply sorting
             data = ApplySorting(data, sortColumn, sortColumnDirection, modelType);

            return data;
        }
        private IQueryable FilterPropertiesBasedOnViewModel(IQueryable data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        {
            // Assuming your dbContext has a property for each entity type
            var dbSetProperty = _dbContext.GetType().GetProperty(modelType.Name + "es");

            if (dbSetProperty == null)
            {
                throw new NotSupportedException($"Model type '{modelType.Name}' is not supported.");
            }

            var entityType = dbSetProperty.PropertyType.GetGenericArguments().First();



            var joinProperties = modelTypeMapping[modelType.Name];

            // Use dynamic LINQ to create the projection
            var result = data.Join(GetTypedDbSet(entityType), "Oid", "Name", "DefaultIfEmpty()");

            // Select properties dynamically
            var selectProperties = GetSelectProperties(entityType, joinProperties);
            var selectExpression = $"new ({string.Join(", ", selectProperties)})";

            return result.Select(selectExpression);
        }

        private IEnumerable<string> GetSelectProperties(Type entityType, (Type, Type) joinProperties)
        {
            var properties = entityType.GetProperties();

            // Assuming joinProperties is a tuple of types (EntityPropertyType, ViewModelPropertyType)
            return properties.Select(p =>
            {
                var propertyName = p.Name;

                // Check if the property exists in the ViewModel
                var viewModelProperty = joinProperties.Item2.GetProperty(propertyName);

                // If the property exists, use it; otherwise, use the original property name
                return viewModelProperty != null ? $"b.{propertyName} as {viewModelProperty.Name}" : $"b.{propertyName}";
            });
        }


        //private IQueryable FilterPropertiesBasedOnViewModel(IQueryable data, Type modelType, Type viewModelType, Dictionary<string, (Type, Type)> modelTypeMapping)
        //{
        //    switch (modelType.Name)
        //    {
        //        case "Branch":
        //            return BranchProperties(data);
        //        case "Country":
        //            return CountryProperties(data);
        //        case "Province":
        //            return ProvinceProperties(data);
        //        case "Resource":
        //            return ResourceProperties(data);
        //        case "SubProvince":
        //            return SubProvinceProperties(data);
        //        case "KappSetting":
        //            return KappSettingProperties(data);
        //        case "KioskApplication":
        //            return KioskApplicationProperties(data);
        //        case "KioskApplicationType":
        //            return KioskApplicationTypeProperties(data);
        //        case "Language":
        //            return LanguageProperties(data);
        //        default:
        //            throw new NotSupportedException($"Model type '{modelType.Name}' is not supported.");
        //    }
        //}

        //private IQueryable BranchProperties(IQueryable data)
        //{
        //    return from b in _dbContext.Branches
        //           join a in _dbContext.Accounts on b.Account equals a.Oid into accountGroup
        //           from account in accountGroup.DefaultIfEmpty()
        //           join c in _dbContext.Countries on b.Country equals c.Oid into countryGroup
        //           from country in countryGroup.DefaultIfEmpty()
        //           join p in _dbContext.Provinces on b.Province equals p.Oid into provinceGroup
        //           from province in provinceGroup.DefaultIfEmpty()
        //           join sp in _dbContext.SubProvinces on b.SubProvince equals sp.Oid into subProvinceGroup
        //           from subProvince in subProvinceGroup.DefaultIfEmpty()
        //           join tp in _dbContext.TicketPoolProfiles on b.TicketPoolProfile equals tp.Oid into ticketPoolProfileGroup
        //           from ticketPoolProfile in ticketPoolProfileGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               b.Name,
        //               Account = account != null ? account.Name : null,
        //               Country = country != null ? country.Name : null,
        //               Province = province != null ? province.Name : null,
        //               SubProvince = subProvince != null ? subProvince.Name : null,
        //               b.BranchCode,
        //               b.Terminal,
        //               b.Area,
        //               b.Address,
        //               b.Address2,
        //               TicketPoolProfile = ticketPoolProfile != null ? ticketPoolProfile.Name : null,
        //           };
        //}

        //private IQueryable CountryProperties(IQueryable data)
        //{
        //    return from c in _dbContext.Countries
        //           select new
        //           {
        //               c.Name,
        //               c.Mask,
        //               c.Sequence,
        //               c.PhoneCode,
        //               c.LangCode,
        //               c.Logo,
        //               c.Oid
        //           };
        //}

        //private IQueryable ProvinceProperties(IQueryable data)
        //{
        //    return from p in _dbContext.Provinces
        //           join a in _dbContext.Countries on p.Country equals a.Oid into countryGroup
        //           from country in countryGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               p.Name,
        //               Country = country != null ? country.Name : null,
        //               p.Oid
        //           };
        //}

        //private IQueryable ResourceProperties(IQueryable data)
        //{
        //    return from r in _dbContext.Resources
        //           join a in _dbContext.Accounts on r.Account equals a.Oid into accountGroup
        //           from account in accountGroup.DefaultIfEmpty()
        //           join l in _dbContext.Languages on r.Language equals l.Oid into languageGroup
        //           from language in languageGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               r.Parameter,
        //               r.ParameterValue,
        //               r.Description,
        //               Language = language != null ? language.Name : null,
        //               Account = account != null ? account.Name : null,
        //               r.Oid
        //           };
        //}

        //private IQueryable SubProvinceProperties(IQueryable data)
        //{
        //    return from sp in _dbContext.SubProvinces
        //           join p in _dbContext.Provinces on sp.Province equals p.Oid into provinceGroup
        //           from province in provinceGroup.DefaultIfEmpty()
        //           join c in _dbContext.Countries on sp.Country equals c.Oid into countryGroup
        //           from country in countryGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               sp.Name,
        //               Province = province != null ? province.Name : null,
        //               Country = country != null ? country.Name : null,
        //               sp.Oid
        //           };
        //}

        //private IQueryable KappSettingProperties(IQueryable data)
        //{
        //    return from k in _dbContext.KappSettings
        //           join a in _dbContext.Accounts on k.Account equals a.Oid into accountGroup
        //           from account in accountGroup.DefaultIfEmpty()
        //           join ka in _dbContext.KioskApplications on k.KioskApplication equals ka.Oid into kioskAppGroup
        //           from kioskApp in kioskAppGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               k.Parameter,
        //               k.ParameterValue,
        //               k.Description,
        //               Account = account != null ? account.Name : null,
        //               k.CreatedDateUtc,
        //               k.ModifiedDate,
        //               k.CacheTimeout,
        //               KappName = kioskApp != null ? kioskApp.KappName : null,
        //               k.Oid
        //           };
        //}

        //private IQueryable KioskApplicationProperties(IQueryable data)
        //{
        //    return from ka in _dbContext.KioskApplications
        //           join a in _dbContext.Accounts on ka.Account equals a.Oid into accountGroup
        //           from account in accountGroup.DefaultIfEmpty()
        //           join b in _dbContext.Branches on ka.Branch equals b.Oid into branchGroup
        //           from branch in branchGroup.DefaultIfEmpty()
        //           join kw in _dbContext.KappWorkflows on ka.KappWorkflow equals kw.Oid into kappWorkflowGroup
        //           from kappWorkflow in kappWorkflowGroup.DefaultIfEmpty()
        //           join kat in _dbContext.KioskApplicationTypes on ka.KioskApplicationType equals kat.Oid into kioskAppTypeGroup
        //           from kioskAppType in kioskAppTypeGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               KappName = ka.KappName,
        //               Account = account != null ? account.Name : null,
        //               Branch = branch != null ? branch.Name : null,
        //               KappWorkflow = kappWorkflow != null ? kappWorkflow.Name : null,
        //               ka.HwId,
        //               KioskApplicationType = kioskAppType != null ? kioskAppType.Name : null,
        //               ka.ModifiedDate,
        //               ka.Active,
        //               ka.Oid
        //           };
        //}

        //private IQueryable KioskApplicationTypeProperties(IQueryable data)
        //{
        //    return from kat in _dbContext.KioskApplicationTypes
        //           join a in _dbContext.Accounts on kat.Account equals a.Oid into accountGroup
        //           from account in accountGroup.DefaultIfEmpty()
        //           select new
        //           {
        //               kat.Name,
        //               kat.Code,
        //               kat.QorchAppType,
        //               Account = account != null ? account.Name : null,
        //               kat.Oid
        //           };
        //}

        //private IQueryable LanguageProperties(IQueryable data)
        //{
        //    return from l in _dbContext.Languages
        //           select new
        //           {
        //               l.Name,
        //               l.EnglishName,
        //               l.LocalName,
        //               l.CultureInfo,
        //               l.LangCode,
        //               l.IsDefault,
        //               l.Logo,
        //               l.Oid
        //           };
        //}



        #endregion
    }

}
