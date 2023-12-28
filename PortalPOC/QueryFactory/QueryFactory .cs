using Microsoft.EntityFrameworkCore;
using PortalPOC.Helpers;
using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public class QueryFactory : IQueryFactory
    {
        public IQueryable SelectQuery(Type modelType, QuavisQorchAdminEasyTestContext _dbContext)
        {
            switch (modelType.Name)
            {
                case "Branch":
                    return QueriesHelper.BranchProperties(_dbContext);
                case "Country":
                    return QueriesHelper.CountryProperties(_dbContext);
                case "Language":
                    return QueriesHelper.LanguageProperties(_dbContext);
                case "Province":
                    return QueriesHelper.ProvinceProperties(_dbContext);

                case "SubProvince":
                    return QueriesHelper.SubProvinceProperties(_dbContext);
                case "KappSetting":
                    return QueriesHelper.KappSettingProperties(_dbContext);
                case "KioskApplication":
                    return QueriesHelper.KioskApplicationProperties(_dbContext);
                case "KioskApplicationType":
                    return QueriesHelper.KioskApplicationTypeProperties(_dbContext);
                case "KappWorkflow":
                    return QueriesHelper.KappWorkFlowProperties(_dbContext);
                case "Design":
                    return QueriesHelper.DesignProperties(_dbContext);
                case "Desk":
                    return QueriesHelper.DeskProperties(_dbContext);
                case "Macro":
                    return QueriesHelper.MacroProperties(_dbContext);
                case "MacroRule":
                    return QueriesHelper.MacroRuleProperties(_dbContext);
                case "TicketPool":
                    return QueriesHelper.TicketPoolProperties(_dbContext);
                case "Ticket":
                    return QueriesHelper.TicketProperties(_dbContext);
                case "TicketState":
                    return QueriesHelper.TicketStateProperties(_dbContext);
                case "Segment":
                    return QueriesHelper.SegmentProperties(_dbContext);

                case "Resource":
                    return QueriesHelper.ResourceProperties(_dbContext);

                default:
                    throw new NotSupportedException($"Model type '{modelType.Name}' is not supported.");
            }
        }
    }
}
