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
    /// <summary>
    /// There are 2 Type of Kiosks Ticket Kiosk , and  a Display Kiosk which is mainly a Televison
    /// Controls the display functionalities for Display kiosks, managing the presentation of waiting tickets and screen refreshes.
    /// </summary>
    public class DisplayController : Controller
    {
        private readonly HwManager _hwman;
        private readonly ApiService _apiService;

        public DisplayController(ApiService httpService, HwManager hwman)
        {
            _hwman = hwman;
            _apiService = httpService;

        }

        /// <summary>
        /// Displays the main view of the Display kiosks, optionally just refreshing the waiting list.
        /// </summary>
        /// <param name="justRefresh">Indicates whether only the waiting list should be refreshed.</param>
        /// <returns>A view or partial view with the display and design model.</returns>
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

        /// <summary>
        /// Refreshes the waiting list without reloading the entire page.
        /// </summary>
        /// <returns>A task that results in the updated display and design model with the new waiting list.</returns>
        private async Task<DisplayAndDesignModel> RefreshWaitingList()
        {
            var viewModel = new DisplayAndDesignModel
            {
                DesPageData = Session.displayAndDesignModel.DesPageData,
                WaitingTickets = await _apiService.GetGenericResponse<List<TicketDto>>($"api/Kiosk/GetInServiceTickets/{KioskContext.KioskHwId}")

            };

            return viewModel;
        }

        /// <summary>
        /// Initializes the homepage by fetching the necessary design data and waiting tickets.
        /// </summary>
        /// <returns>A task that results in the display and design model for the homepage.</returns>
        private async Task<DisplayAndDesignModel> InitHomepage()
        {
            var viewModel = new DisplayAndDesignModel
            {
                DesPageData = await GetDesignData(KioskContext.KioskHwId, Step.DisplayScreen.ToString()),
                WaitingTickets = await _apiService.GetGenericResponse<List<TicketDto>>($"api/Kiosk/GetInServiceTickets/{KioskContext.KioskHwId}")

            };
            KioskContext.Languages = await GetLanguageList();
            KioskContext.Resources = await GetResourceList();
            KioskContext.CurrentLanguage = KioskContext.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

            Session.displayAndDesignModel = viewModel;

            return viewModel;
        }

        /// <summary>
        /// Fetches design-related data for a specific step of the kiosk's operation.
        /// </summary>
        /// <param name="hwId">The hardware ID of the kiosk.</param>
        /// <param name="step">The operation step for which design data is needed.</param>
        /// <returns>A task that results in the design data for the specified step.</returns>
        private async Task<DesPageData> GetDesignData(string hwId, string step)
        {
            return await _apiService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }

        /// <summary>
        /// Retrieves a list of resources available for the kiosk interface.
        /// </summary>
        /// <returns>A task that results in a list of resources.</returns>
        private async Task<List<Resource>> GetResourceList()
        {
            return await _apiService.GetDesignResponse<List<Resource>>($"api/Kiosk/GetResourceList");

        }



        /// <summary>
        /// Asynchronously retrieves a list of languages supported by the kiosk.
        /// </summary>
        /// <returns>A task that results in a list of languages.</returns>
        private async Task<List<Language>> GetLanguageList()
        {
            return await _apiService.GetDesignResponse<List<Language>>($"api/Kiosk/GetLanguageList");
        }

        /// <summary>
        /// Checks and returns the current status of the kiosk hardware.
        /// </summary>
        /// <returns>An Ok response with the hardware status.</returns>
        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }

    }
}
