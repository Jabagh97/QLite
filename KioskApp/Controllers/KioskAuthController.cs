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
        [HttpGet]

        public async Task<IActionResult> AuthenticateAsync()
        {
            try
            {
                var kioskId = _configuration.GetValue<string>("KioskID");

                var response = await _httpClient.GetAsync($"api/Kiosk/GetKioskByHwID/{kioskId}");

                if (response.IsSuccessStatusCode)
                {
                    var kioskData = await response.Content.ReadAsStringAsync();
                    return Ok(kioskData);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request."); // Handle unexpected errors
            }
        }


    }
}
