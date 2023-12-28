using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace PortalPOC.Helpers
{
    public static class QueriesHelper
    {
        public static IQueryable BranchProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from b in _dbContext.Branches
                   join a in _dbContext.Accounts on b.Account equals a.Oid into accountGroup
            from account in accountGroup.DefaultIfEmpty()
                   join c in _dbContext.Countries on b.Country equals c.Oid into countryGroup
            from country in countryGroup.DefaultIfEmpty()
                   join p in _dbContext.Provinces on b.Province equals p.Oid into provinceGroup
                   from province in provinceGroup.DefaultIfEmpty()
                   join sp in _dbContext.SubProvinces on b.SubProvince equals sp.Oid into subProvinceGroup
                   from subProvince in subProvinceGroup.DefaultIfEmpty()
                   join tp in _dbContext.TicketPoolProfiles on b.TicketPoolProfile equals tp.Oid into ticketPoolProfileGroup
                   from ticketPoolProfile in ticketPoolProfileGroup.DefaultIfEmpty()
                   select new
                   {
                       b.Name,
                       Account = account != null ? account.Name : null,
                       Country = country != null ? country.Name : null,
                       Province = province != null ? province.Name : null,
                       SubProvince = subProvince != null ? subProvince.Name : null,
                       b.BranchCode,
                       b.Terminal,
                       b.Area,
                       b.Address,
                       b.Address2,
                       TicketPoolProfile = ticketPoolProfile != null ? ticketPoolProfile.Name : null,
                   };
        }

        public static IQueryable CountryProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from c in _dbContext.Countries
                   select new
                   {
                       c.Name,
                       c.Mask,
                       c.Sequence,
                       c.PhoneCode,
                       c.LangCode,
                       c.Logo,
                       c.Oid
                   };
        }

        public static IQueryable ProvinceProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from p in _dbContext.Provinces
                   join a in _dbContext.Countries on p.Country equals a.Oid into countryGroup
                   from country in countryGroup.DefaultIfEmpty()
                   select new
                   {
                       p.Name,
                       Country = country != null ? country.Name : null,
                       p.Oid
                   };
        }

        public static IQueryable ResourceProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from r in _dbContext.Resources
                   join a in _dbContext.Accounts on r.Account equals a.Oid into accountGroup
            from account in accountGroup.DefaultIfEmpty()
                   join l in _dbContext.Languages on r.Language equals l.Oid into languageGroup
                   from language in languageGroup.DefaultIfEmpty()
                   select new
                   {
                       r.Parameter,
                       r.ParameterValue,
                       r.Description,
                       Language = language != null ? language.Name : null,
                       Account = account != null ? account.Name : null,
                       r.Oid
                   };
        }

        public static IQueryable SubProvinceProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from sp in _dbContext.SubProvinces
                   join p in _dbContext.Provinces on sp.Province equals p.Oid into provinceGroup
                   from province in provinceGroup.DefaultIfEmpty()
                   join c in _dbContext.Countries on sp.Country equals c.Oid into countryGroup
                   from country in countryGroup.DefaultIfEmpty()
                   select new
                   {
                       sp.Name,
                       Province = province != null ? province.Name : null,
                       Country = country != null ? country.Name : null,
                       sp.Oid
                   };
        }

        public static IQueryable KappSettingProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from k in _dbContext.KappSettings
                   join a in _dbContext.Accounts on k.Account equals a.Oid into accountGroup
                   from account in accountGroup.DefaultIfEmpty()
                   join ka in _dbContext.KioskApplications on k.KioskApplication equals ka.Oid into kioskAppGroup
                   from kioskApp in kioskAppGroup.DefaultIfEmpty()
                   select new
                   {
                       k.Parameter,
                       k.ParameterValue,
                       k.Description,
                       Account = account != null ? account.Name : null,
                       k.CreatedDateUtc,
                       k.ModifiedDate,
                       k.CacheTimeout,
                       KappName = kioskApp != null ? kioskApp.KappName : null,
                       k.Oid
                   };
        }

        public static IQueryable KioskApplicationProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from ka in _dbContext.KioskApplications
                   join a in _dbContext.Accounts on ka.Account equals a.Oid into accountGroup
                   from account in accountGroup.DefaultIfEmpty()
                   join b in _dbContext.Branches on ka.Branch equals b.Oid into branchGroup
                   from branch in branchGroup.DefaultIfEmpty()
                   join kw in _dbContext.KappWorkflows on ka.KappWorkflow equals kw.Oid into kappWorkflowGroup
                   from kappWorkflow in kappWorkflowGroup.DefaultIfEmpty()
                   join kat in _dbContext.KioskApplicationTypes on ka.KioskApplicationType equals kat.Oid into kioskAppTypeGroup
                   from kioskAppType in kioskAppTypeGroup.DefaultIfEmpty()
                   select new
                   {
                       KappName = ka.KappName,
                       Account = account != null ? account.Name : null,
                       Branch = branch != null ? branch.Name : null,
                       KappWorkflow = kappWorkflow != null ? kappWorkflow.Name : null,
                       ka.HwId,
                       KioskApplicationType = kioskAppType != null ? kioskAppType.Name : null,
                       ka.ModifiedDate,
                       ka.Active,
                       ka.Oid
                   };
        }

        public static IQueryable KioskApplicationTypeProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from kat in _dbContext.KioskApplicationTypes
                   join a in _dbContext.Accounts on kat.Account equals a.Oid into accountGroup
                   from account in accountGroup.DefaultIfEmpty()
                   select new
                   {
                       kat.Name,
                       kat.Code,
                       kat.QorchAppType,
                       Account = account != null ? account.Name : null,
                       kat.Oid
                   };
        }

        public static IQueryable LanguageProperties(QuavisQorchAdminEasyTestContext _dbContext)
        {
            return from l in _dbContext.Languages
                   select new
                   {
                       l.Name,
                       l.EnglishName,
                       l.LocalName,
                       l.CultureInfo,
                       l.LangCode,
                       l.IsDefault,
                       l.Logo,
                       l.Oid
                   };
        }
    }
}
