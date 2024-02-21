using Microsoft.AspNetCore.Mvc;
using QLite.Data;

namespace KioskApp.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Index(Guid SegmentOid)
        {
            var services = FetchServiceTypesFromAPI(); 

            ViewBag.Services = services;

            return PartialView("Views/Home/Services.cshtml");
        }

        private List<ServiceType> FetchServiceTypesFromAPI()
        {

            return new List<ServiceType>
        {
            new ServiceType { Oid = new Guid() , Name = "Service 1" },
            new ServiceType { Oid = new Guid(), Name = "Service 2" },
            new ServiceType { Oid = new Guid(), Name = "Service 3" },
        };
        }
    }
}
