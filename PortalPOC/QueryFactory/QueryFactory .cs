
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Helpers;
using PortalPOC.Models;
using PortalPOC.ViewModels.Branch;
using PortalPOC.ViewModels.Country;
using PortalPOC.ViewModels.Design;
using PortalPOC.ViewModels.Desk;
using PortalPOC.ViewModels.KappRole;
using PortalPOC.ViewModels.KappSetting;
using PortalPOC.ViewModels.KappWorkflow;
using PortalPOC.ViewModels.KioskApplication;
using PortalPOC.ViewModels.KioskApplicationType;
using PortalPOC.ViewModels.Language;
using PortalPOC.ViewModels.Macro;
using PortalPOC.ViewModels.MacroRule;
using PortalPOC.ViewModels.Province;
using PortalPOC.ViewModels.Resource;
using PortalPOC.ViewModels.Segment;
using PortalPOC.ViewModels.SubProvince;
using PortalPOC.ViewModels.Ticket;
using PortalPOC.ViewModels.TicketPool;
using PortalPOC.ViewModels.TicketState;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace PortalPOC.QueryFactory
{
  
    public class QueryFactory : IQueryFactory
    {

        public IQueryable GetEntityProperties<TModel, TViewModel>(QuavisQorchAdminEasyTestContext _dbContext) where TModel : class
        {
            var query = _dbContext.Set<TModel>().AsQueryable();
            var modelProperties = typeof(TModel).GetProperties();
            var viewModelProperties = typeof(TViewModel).GetProperties();

            var selectProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => vp.Name == mp.Name));

            var leftJoinProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => mp.Name.Contains(vp.Name)) && QueriesHelper.IsNavigationProperty(mp));

            var combinedProperties = selectProperties
                .Select(sp => leftJoinProperties.FirstOrDefault(lp => lp.Name.Contains(sp.Name)) ?? sp);

            var selectExpressions = combinedProperties.Select(property =>
            {
                var propertyToJoin = property.Name switch
                {
                    "KioskApplication" => "KappName",
                    _ => "Name"
                };

                var navigationProperty = property.Name.Contains("Navigation")
                    ? property.Name.Replace("Navigation", "")
                    : null;

                return !navigationProperty.IsNullOrEmpty()
                    ? $"{property.Name} != null ? {property.Name}.{propertyToJoin} : null as {navigationProperty}"
                    : $"{property.Name} as {property.Name}";
            });

            var selectProjection = string.Join(", ", selectExpressions);

            return query.Select($"new ({selectProjection})");
        }


        public IQueryable SelectQuery(Type modelType, QuavisQorchAdminEasyTestContext _dbContext)
        {
            return modelType.Name switch
            {
                "Branch" => GetEntityProperties<Branch, BranchViewModel>(_dbContext),
                "Country" => GetEntityProperties<Country, CountryViewModel>(_dbContext),
                "Language" => GetEntityProperties<Language, LanguageViewModel>(_dbContext),
                "Province" => GetEntityProperties<Province, ProvinceViewModel>(_dbContext),
                "SubProvince" => GetEntityProperties<SubProvince, SubProvinceViewModel>(_dbContext),
                "KappSetting" => GetEntityProperties<KappSetting, KappSettingViewModel>(_dbContext),
                "KioskApplication" => GetEntityProperties<KioskApplication, KioskApplicationViewModel>(_dbContext),
                "KioskApplicationType" => GetEntityProperties<KioskApplicationType, KioskApplicationTypeViewModel>(_dbContext),
                "Design" => GetEntityProperties<Design, DesignViewModel>(_dbContext),
                "KappWorkflow" => GetEntityProperties<KappWorkflow, KappWorkflowViewModel>(_dbContext),
                "Desk" => GetEntityProperties<Desk, DeskViewModel>(_dbContext),
                "Macro" => GetEntityProperties<Macro, MacroViewModel>(_dbContext),
                "MacroRule" => GetEntityProperties<MacroRule, MacroRuleViewModel>(_dbContext),
                "TicketPool" => GetEntityProperties<TicketPool, TicketPoolViewModel>(_dbContext),
                "Ticket" => GetEntityProperties<Ticket, TicketViewModel>(_dbContext),
                "TicketState" => GetEntityProperties<TicketState, TicketStateViewModel>(_dbContext),
                "Segment" => GetEntityProperties<Segment, SegmentViewModel>(_dbContext),
                "Resource" => GetEntityProperties<Resource, ResourceViewModel>(_dbContext),
                // Add more cases as needed...
                _ => throw new NotSupportedException($"Model type '{modelType.Name}' is not supported.")
            };
        }



    
    }
}
