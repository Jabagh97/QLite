
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

        public IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext _dbContext)
        {
            if (data == null)
            {
                return Enumerable.Empty<object>().AsQueryable();
            }

            var query = data;
            var modelProperties = modelType.GetProperties();
            var viewModelProperties = viewModelType.GetProperties();

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

    }
}
