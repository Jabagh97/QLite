using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static QLite.Data.Models.Enums;
using System.Security.Claims;
using System.Text;
using QLite.Data.Services;
using QLite.Data.Dtos;

namespace KioskApp.Controllers
{
    public class KioskAuthController : Controller
    {
        private readonly IApiService _apiService;
        public KioskAuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] string KioskID)
        {
            var restClient = await _apiService.GetAsync("api/Kiosk/GetKioskByID");

          return Ok(restClient);
        }

    }
}
