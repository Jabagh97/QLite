using AdminPortal.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLiteDataApi.Constants;
using System.Xml.Serialization;

namespace AdminPortal.Controllers
{
    public class ReportsController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ReportsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var apiBase = _configuration.GetValue<string>("APIBase");
            if (string.IsNullOrEmpty(apiBase))
                throw new ArgumentException("APIBase configuration is missing or invalid.", nameof(apiBase));

            _httpClient.BaseAddress = new Uri(apiBase);
        }


        public IActionResult Index(string reportType = "")
        {
            ViewBag.PageTitle = reportType;
            return View(reportType);
        }


        [HttpGet]
        public async Task<IActionResult> GetTicketStateReport(string? StartDate,string EndDate)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Admin/GetTicketStateReport/{StartDate}/{EndDate}");
                return response.IsSuccessStatusCode
                    ? Ok(await response.Content.ReadAsStringAsync())
                    : StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
       
        }
    }
}
