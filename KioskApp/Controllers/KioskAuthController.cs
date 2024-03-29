using Microsoft.AspNetCore.Mvc;
using static QLite.Data.Models.Enums;
using QLite.Data.Services;
using QLite.Data.Dtos;
using QLite.Data.CommonContext;
using Serilog;
using KioskApp.Constants;
using KioskApp.Models;
using QLite.DesignComponents;

namespace KioskApp.Controllers
{
    /// <summary>
    /// Handles authentication processes for kiosks, including initial validation and routing based on kiosk configuration and kiosk type.
    /// </summary>
    public class KioskAuthController : Controller
    {
        private readonly ApiService _apiService;

        public KioskAuthController(ApiService apiService)
        {
            _apiService = apiService;

        }
        /// <summary>
        /// Authenticates the kiosk by its hardware ID and directs to the appropriate workflow based on the chosen workflow by the admin portal.
        /// </summary>
        /// <returns>A redirection to the correct view or an error view.</returns>
        public async Task<IActionResult> AuthenticateAsync()
        {
            try
            {
                var kioskId = KioskContext.Config.GetValue<string>("KioskID");
                var kioskData = await _apiService.GetGenericResponse<KioskDto>($"api/Kiosk/GetKioskByHwID/{kioskId}");


                if (kioskData == null || kioskId == null)
                {
                    Log.Error($"{Errors.KioskError} No kiosk data found");

                    var errorViewModel = new ErrorViewModel
                    {
                        Error = Errors.AuthenticatioError ,
                        Solution = Errors.AuthenticatioSolution
                    };

                    return View("KioskDownError", errorViewModel);
                }

                KioskContext.KioskConfig = kioskData;

                switch (kioskData.KioskType)
                {
                    case (int)KioskType.Kiosk:
                        return await HandleKioskType(kioskData, kioskId);

                    case (int)KioskType.Display:
                        return RedirectToAction("Index", "Display");

                    default:
                        Log.Error($"{Errors.KioskError} Unhandled kiosk type: {kioskData.KioskType}");
                        var errorViewModel = new ErrorViewModel
                        {
                            Error = Errors.KioskTypeError,
                            Solution = Errors.KioskTypeSolution
                        };

                        return View("KioskDownError", errorViewModel);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{Errors.KioskError} An error occurred while processing the kiosk authentication request.");
                var errorViewModel = new ErrorViewModel
                {
                    Error = Errors.AuthenticatioError,
                    Solution = Errors.AuthenticatioSolution
                };

                return View("KioskDownError", errorViewModel);
            }
        }

        /// <summary>
        /// Handles the redirection based on the kiosk's workflow configuration.
        /// </summary>
        /// <param name="kioskData">The data object representing the kiosk's current state and configuration.</param>
        /// <param name="kioskId">The hardware ID of the kiosk.</param>
        /// <returns>A redirection to the appropriate controller and action.</returns>
        private async Task<IActionResult> HandleKioskType(KioskDto kioskData, string kioskId)
        {
            switch (kioskData.WorkFlowType)
            {
                case (int)WorkFlowType.WelcomeSegmentService:
                    return RedirectToAction("Index", "Kiosk");

                case (int)WorkFlowType.SegmentService:
                    return RedirectToAction("GetSegmentView", "Kiosk", new { hwId = kioskId, asFirstPage = true });

                case (int)WorkFlowType.OnlyServices:
                    var defaultSegment = await _apiService.GetGenericResponse<Guid>($"api/Kiosk/GetDefaultSegment");
                    if (defaultSegment == Guid.Empty)
                    {
                        Log.Error($"{Errors.KioskError} Default segment could not be retrieved.");
                        var errorViewModel = new ErrorViewModel
                        {
                            Error = Errors.DefaultSegmentError,
                            Solution = Errors.DefaultSegmentSolution
                        };

                        return View("KioskDownError", errorViewModel);
                    }
                    return RedirectToAction("GetServiceView", "Kiosk", new { segmentOid = defaultSegment, hwId = kioskId, asFirstPage = true });

                default:
                    Log.Information($"Unhandled workflow type: {kioskData.WorkFlowType}, default workflow will be used");
                    return RedirectToAction("Index", "Kiosk");
            }
        }

        /// <summary>
        /// Authenticates the kiosk for WebSocket connections by retrieving its configuration for the JavaScript part of the app to Initialize WebSocket Conneection.
        /// </summary>
        /// <returns>An Ok result with the kiosk's data or an error response.</returns>

        [HttpGet]
        public IActionResult AuthenticateForWebSocket()
        {
            try
            {
                var kioskData = KioskContext.KioskConfig;
                return Ok(kioskData);

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{Errors.KioskError} An error occurred while processing the kiosk authentication request.");

                return StatusCode(500, "An error occurred while processing the request.");
            }
        }





    }
}
