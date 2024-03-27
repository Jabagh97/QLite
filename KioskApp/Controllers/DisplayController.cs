using KioskApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.CommonContext;
using QLite.Data.Services;
using QLite.DesignComponents;
using Quavis.QorchLite.Hwlib;
using Serilog;
using static QLite.Data.Models.Enums;

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
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await InitHomepage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading the Kiosk page.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }
        private async Task<DesPageData> InitHomepage()
        {


            DesPageData desPageData = await GetDesignData(CommonCtx.KioskHwId, Step.DisplayScreen.ToString());
            

            CommonCtx.Languages = await GetLanguageList();
            CommonCtx.Resources = await GetResourceList();
            CommonCtx.CurrentLanguage = CommonCtx.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

            return desPageData;
        }
        private async Task<DesPageData> GetDesignData(string hwId, string step)
        {
            return await _apiService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }
        private async Task<List<Resource>> GetResourceList()
        {
            return await _apiService.GetDesignResponse<List<Resource>>($"api/Kiosk/GetResourceList");

        }

        private async Task<List<Language>> GetLanguageList()
        {
            return await _apiService.GetDesignResponse<List<Language>>($"api/Kiosk/GetLanguageList");
        }

        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }

    }
}
