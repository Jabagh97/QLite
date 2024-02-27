using KioskApp.Models;
using Microsoft.AspNetCore.Mvc;
using Quavis.QorchLite.Hwlib;
using System.Diagnostics;

namespace KioskApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        HwManager _hwman;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HwManager hwman)
        {
            _logger = logger;
            _configuration = configuration;
            _hwman = hwman;

        }

        public IActionResult Index()
        {
            string kioskId = _configuration.GetValue<string>("KioskID");
            ViewData["KioskID"] = kioskId;

            return View();
        }
       
        public virtual IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}