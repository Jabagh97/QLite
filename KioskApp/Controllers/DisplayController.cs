using KioskApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;
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
        public async Task<IActionResult> Index(bool justRefresh = false)
        {
            try
            {
                DisplayAndDesignModel viewModel;

                if (justRefresh)
                {
                    viewModel = await RefreshWaitingList();
                    return PartialView("~/Views/Display/Index.cshtml", viewModel);
                }
                viewModel = await InitHomepage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading the Kiosk page.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }

        private async Task<DisplayAndDesignModel> RefreshWaitingList()
        {
            var viewModel = new DisplayAndDesignModel
            {
                DesPageData = Session.displayAndDesignModel.DesPageData,
                WaitingTickets = await _apiService.GetGenericResponse<List<TicketDto>>($"api/Kiosk/GetInServiceTickets/{CommonCtx.KioskHwId}")

            };

            return viewModel;
        }
        private async Task<DisplayAndDesignModel> InitHomepage()
        {
            var viewModel = new DisplayAndDesignModel
            {
                DesPageData = await GetDesignData(CommonCtx.KioskHwId, Step.DisplayScreen.ToString()),
                WaitingTickets = await _apiService.GetGenericResponse<List<TicketDto>>($"api/Kiosk/GetInServiceTickets/{CommonCtx.KioskHwId}")

            };
            CommonCtx.Languages = await GetLanguageList();
            CommonCtx.Resources = await GetResourceList();
            CommonCtx.CurrentLanguage = CommonCtx.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

            Session.displayAndDesignModel = viewModel;

            return viewModel;
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
