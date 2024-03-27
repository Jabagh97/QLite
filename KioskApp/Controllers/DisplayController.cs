using Microsoft.AspNetCore.Mvc;
using QLite.Data.Services;
using Quavis.QorchLite.Hwlib;

namespace KioskApp.Controllers
{
    public class DisplayController : Controller
    {
        private readonly HwManager _hwman;
        private readonly ApiService _apiService;

        public DisplayController(ApiService httpService, HwManager hwman)
        {
            _hwman = hwman;
            _apiService = httpService;

        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }

    }
}
