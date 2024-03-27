using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static QLite.Data.Models.Enums;
using System.Security.Claims;
using System.Text;
using QLite.Data.Services;
using QLite.Data.Dtos;
using Microsoft.Extensions.Configuration;
using QLite.Data.CommonContext;
using Quavis.QorchLite.Hwlib;
using System.Diagnostics;

namespace KioskApp.Controllers
{
    public class KioskAuthController : Controller
    {
        private readonly ApiService _apiService;

        public KioskAuthController(ApiService apiService)
        {
            _apiService = apiService;

        }

        public async Task<IActionResult> AuthenticateAsync()
        {
            try
            {
                var kioskId = CommonCtx.Config.GetValue<string>("KioskID");

                var kioskData = await _apiService.GetGenericResponse<KioskDto>($"api/Kiosk/GetKioskByHwID/{kioskId}");


                // Assuming KioskType is an enum and kioskData.KioskType is its integer representation
                if ((KioskType)kioskData.KioskType == KioskType.Kiosk)
                {
                    return RedirectToAction("Index", "Kiosk");
                }
                else if ((KioskType)kioskData.KioskType == KioskType.Display)
                {
                    return RedirectToAction("Index", "Display");
                }

                return StatusCode(500, "Kiosk Not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request."); // Handle unexpected errors
            }
        }


        [HttpGet]
        public async Task<IActionResult> AuthenticateForWebSocket()
        {
            try
            {
                var kioskId = CommonCtx.Config.GetValue<string>("KioskID");

                var kioskData = await _apiService.GetGenericResponse<KioskDto>($"api/Kiosk/GetKioskByHwID/{kioskId}");


                return Ok(kioskData);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request."); // Handle unexpected errors
            }
        }




    
    }
}
