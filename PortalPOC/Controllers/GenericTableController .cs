using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using PortalPOC.Services;
using PortalPOC.ViewModals.Branch;

using PortalPOC.ViewModals.Language;
using PortalPOC.ViewModals.Country;
using PortalPOC.ViewModals.Account;
using PortalPOC.ViewModals.Appointment;
using PortalPOC.ViewModals.Design;
using PortalPOC.ViewModals.AppointmentSetting;
using PortalPOC.ViewModals.DesignTarget;
using PortalPOC.ViewModals.Desk;
using PortalPOC.ViewModals.KappRole;
using PortalPOC.ViewModals.KappSetting;
using PortalPOC.ViewModals.KappUser;
using PortalPOC.ViewModals.KappWorkflow;
using PortalPOC.ViewModals.KioskApplication;
using PortalPOC.ViewModals.Province;
using PortalPOC.ViewModals.Macro;
using PortalPOC.ViewModals.DeskCreatableService;
using PortalPOC.ViewModals.DeskTransferableService;
using PortalPOC.ViewModals.QorchSession;
using PortalPOC.ViewModals.Resource;
using PortalPOC.ViewModals.Segment;
using PortalPOC.ViewModals.ServiceType;
using PortalPOC.ViewModals.SubProvince;
using PortalPOC.ViewModals.TicketPool;
using PortalPOC.ViewModals.TicketPoolProfile;
using PortalPOC.ViewModals.TicketState;
using PortalPOC.ViewModals.Ticket;
using PortalPOC.ViewModals.MacroRule;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {

        // Define a dictionary to map model names to types
        private readonly Dictionary<string, (Type, Type)> modelTypeMapping = new Dictionary<string, (Type, Type)>
{
    {"Account", (typeof(Account), typeof(AccountViewModel))},
    {"Branch", (typeof(Branch), typeof(BranchViewModel))},
    {"TicketPool", (typeof(TicketPool), typeof(TicketPoolViewModel))},
    {"Ticket", (typeof(Ticket), typeof(TicketViewModel))},
    {"Country", (typeof(Country), typeof(CountryViewModel))},
    {"ServiceType", (typeof(ServiceType), typeof(ServiceTypeViewModel))},
    {"TicketState", (typeof(TicketState), typeof(TicketStateViewModel))},
    {"KappSetting", (typeof(KappSetting), typeof(KappSettingViewModel))},
    {"KioskApplication", (typeof(KioskApplication), typeof(KioskApplicationViewModel))},
    {"KioskApplicationType", (typeof(Branch), typeof(BranchViewModel))},
    {"Language", (typeof(Language), typeof(LanguageViewModel))},
    {"Macro", (typeof(Macro), typeof(MacroViewModel))},
    {"Province", (typeof(Province), typeof(ProvinceViewModel))},
    {"KappRole", (typeof(KappRole), typeof(KappRoleViewModel))},
    {"KappUser", (typeof(KappUser), typeof(KappUserViewModel))},
    {"KappWorkflow", (typeof(KappWorkflow), typeof(KappWorkflowViewModel))},
    {"Resource", (typeof(Resource), typeof(ResourceViewModel))},
    {"Segment", (typeof(Segment), typeof(SegmentViewModel))},
    {"SubProvince", (typeof(SubProvince), typeof(SubProvinceViewModel))},
    {"Desk", (typeof(Desk), typeof(DeskViewModel))},
    {"TicketPoolProfile", (typeof(TicketPoolProfile), typeof(TicketPoolProfileViewModel))},
    {"Appointment", (typeof(Appointment), typeof(AppointmentViewModel))},
    {"AppointmentSetting", (typeof(AppointmentSetting), typeof(AppointmentSettingViewModel))},
    {"Design", (typeof(Design), typeof(DesignViewModel))},
    {"DesignTarget", (typeof(DesignTarget), typeof(DesignTargetViewModel))},
    {"DeskCreatableService", (typeof(DeskCreatableService), typeof(DeskCreatableServiceViewModel))},
    {"DeskTransferableService", (typeof(DeskTransferableService), typeof(DeskTransferableServiceViewModel))},
    {"QorchSession", (typeof(QorchSession), typeof(QorchSessionViewModel))},
     {"MacroRule", (typeof(MacroRule), typeof(MacroRuleViewModel))},

};



        private readonly IDataService _dataService;

        public GenericTableController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index(string modelName)
        {
            if (string.IsNullOrEmpty(modelName) || !modelTypeMapping.TryGetValue(modelName, out var typeTuple))
            {
                return View("Error");
            }

            


            return View(typeTuple.Item2);
        }


        [HttpPost]
        public IActionResult GetData(string modelName)
        {
            // Extract request parameters
            var formData = Request.Form;
            var pageSize = int.Parse(formData["length"]);
            var skip = int.Parse(formData["start"]);
            var searchValue = formData["search[value]"];
            var sortColumn = formData[string.Concat("columns[", formData["order[0][column]"], "][name]")];
            var sortColumnDirection = formData["order[0][dir]"];

            // Check if the model type is valid
            if (modelTypeMapping.TryGetValue(modelName, out var typeTuple))
            {
                Type modelType = typeTuple.Item1;
                Type viewModelType = typeTuple.Item2;

                // Use Type directly to invoke the Set method
                var dbSet = _dataService.GetTypedDbSet(modelType);

                // Query data from the DbSet

                var data = dbSet.Cast<dynamic>()
                                  .Where(e => e.Gcrecord == null);
                                  
               

                // Get filtered and paginated data from DataService
                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, data, searchValue, sortColumn, sortColumnDirection, modelTypeMapping);

                // Paginate the data
                var paginatedData = filteredData.Skip(skip).Take(pageSize).ToList();

                // Get total records count
                //var recordsTotal = filteredData.Count();

                // Prepare JSON response
                // var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

                var jsonData = new {  data = paginatedData };


                return Ok(jsonData);
            }
            else
            {
                return Json(new { data = new List<object>() });
            }
        }


    }

}
