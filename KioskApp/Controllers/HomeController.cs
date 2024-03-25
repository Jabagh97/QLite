using KioskApp.Models;
using KioskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data.CommonContext;
using QLite.Data.Services;
using QLite.DesignComponents;
using Quavis.QorchLite.Hwlib;
using System.Diagnostics;
using System.Net.Http;

namespace KioskApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HwManager _hwman;
        private readonly ApiService _httpService;

        public HomeController(HwManager hwman, ApiService httpService)
        {
           
            _hwman = hwman;
            _httpService = httpService;
            
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeAndDesPageDataViewModel();

            try
            {
                string _kioskHwId = CommonCtx.KioskHwId;

                DesPageData designData = await GetDesignData(_kioskHwId);

                viewModel.DesPageData = designData;
                viewModel.KioskHwId = _kioskHwId;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }

        private async Task<DesPageData> GetDesignData(string hwId)
        {
            var step = "WelcomePage";
            return await _httpService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}