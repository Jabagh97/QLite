using KioskApp.Helpers;
using KioskApp.Models;
using KioskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
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
                CommonCtx.Languages = await GetLanguageList();
                CommonCtx.Resources = await GetResourceList();

                // Check if CurrentLanguage is empty
                if (CommonCtx.CurrentLanguage == Guid.Empty)
                {
                    var firstLanguage = CommonCtx.Languages.FirstOrDefault();
                    // Ensure there's at least one language available
                    if (firstLanguage != null)
                    {
                        CommonCtx.CurrentLanguage = firstLanguage.Oid;
                    }
                }

                string _kioskHwId = CommonCtx.KioskHwId;
                DesPageData designData = await GetDesignData(_kioskHwId);

                viewModel.DesPageData = designData;
                viewModel.KioskHwId = _kioskHwId;


                Session.homeAndDesPageData= viewModel;



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



       


        private async Task<List<Resource>> GetResourceList()
        {
            return await _httpService.GetDesignResponse<List<Resource>>($"api/Kiosk/GetResourceList");

        }

        private async Task<List<Language>> GetLanguageList()
        {
            return await _httpService.GetDesignResponse<List<Language>>($"api/Kiosk/GetLanguageList");
        }

        public async Task<IActionResult> ChangeLanguage(Guid LangID)
        {

            try
            {
                CommonCtx.CurrentLanguage = LangID;
                return PartialView("~/Views/Home/Index.cshtml", Session.homeAndDesPageData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}