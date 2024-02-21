using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static QLite.Data.Models.Enums;
using System.Security.Claims;
using System.Text;
using QLite.Data.Services;
using QLite.Data.Dtos;
using Microsoft.Extensions.Configuration;

namespace KioskApp.Controllers
{
    public class KioskAuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public KioskAuthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] string KioskID)
        {
            var restClient = await _httpClient.GetAsync("api/Kiosk/GetKioskByID");

            return Ok(restClient);
        }

    }
}
