using Microsoft.AspNetCore.Mvc;

namespace KioskApp.Controllers
{
    public class KioskMvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
