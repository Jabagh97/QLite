using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using PortalPOC.Services;
using PortalPOC.TableHelpers;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
       
        // Define a dictionary to map model names to types
        private readonly Dictionary<string, Type> modelTypeMapping = new Dictionary<string, Type>
    {
             {"Account", typeof(Account)},
             {"Branch", typeof(Branch)},
             {"TicketPool", typeof(TicketPool)},
             {"Ticket", typeof(Ticket)},
             {"Country", typeof(Country)},
             {"ServiceType", typeof(ServiceType)},
             {"TicketState", typeof(TicketState)},
             {"KappSetting", typeof(KappSetting)},
             {"KioskApplication", typeof(KioskApplication)},
             {"KioskApplicationType", typeof(Branch)},
             {"Language", typeof(Language)},
             {"Macro", typeof(Macro)},
             {"Province", typeof(Province)},
             {"KappRole", typeof(KappRole)},
             {"KappUser", typeof(KappUser)},
             {"KappWorkflow", typeof(KappWorkflow)},
             {"Resource", typeof(Resource)},
             {"Segment", typeof(Segment)},
             {"SubProvince", typeof(SubProvince)},
             {"Desk", typeof(Desk)},
              {"TicketPoolProfile", typeof(TicketPoolProfile)},

             {"Appointment", typeof(Appointment)},
             {"AppointmentSetting", typeof(AppointmentSetting)},
             {"Design", typeof(Design)},
             {"DesignTarget", typeof(DesignTarget)},
             {"DeskCreatableService", typeof(DeskCreatableService)},
             {"DeskTransferableService", typeof(DeskTransferableService)},
            
             {"QorchSession", typeof(QorchSession)},

    };

        private readonly IDataService _dataService;

        public GenericTableController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index(string modelName)
        {

            if (!modelTypeMapping.TryGetValue(modelName, out Type? modelType))
            {
                return View("Error");
            }

            return View(modelType);
        }
        [HttpPost]
        public IActionResult GetData(string modelName)
        {
            // Extract request parameters
            var pageSize = int.Parse(Request.Form["length"]);
            var skip = int.Parse(Request.Form["start"]);
            var searchValue = Request.Form["search[value]"];
            var sortColumn = Request.Form[string.Concat("columns[", Request.Form["order[0][column]"], "][name]")];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            // Check if the model type is valid
            if (!modelTypeMapping.TryGetValue(modelName, out Type? modelType))
            {
                return Json(new { data = new List<object>() });
            }

            // Use Type directly to invoke the Set method
            var dbSet = _dataService.GetTypedDbSet(modelType);

            // Query data from the DbSet
            var data = ((IEnumerable)dbSet!).Cast<object>();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                data = _dataService.ApplySearchFilter(data, searchValue);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                data = _dataService.ApplySorting(data, sortColumn, sortColumnDirection, modelType);
            }

            // Paginate the data
            var paginatedData = data.Skip(skip).Take(pageSize).ToList();

            // Get total records count
            var recordsTotal = data.Count();

            // Prepare JSON response
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

            return Ok(jsonData);
        }

      




    }

}
