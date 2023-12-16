using Microsoft.AspNetCore.Mvc;
using PortalPOC.Models;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

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

        public GenericTableController(QuavisQorchAdminEasyTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(string modelName)
        {


            if (!modelTypeMapping.TryGetValue(modelName, out Type modelType))
            {
                // Handle the case when the modelName is not found in the mapping
                return NotFound();
            }

            return View(modelType);
        }
    }

}
