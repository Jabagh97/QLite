using AdminPortal.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLite.Data.Services;
using QLiteDataApi.Constants;
using System.Xml.Serialization;

namespace AdminPortal.Controllers
{
    public class ReportsController : Controller
    {

        private readonly ApiService _apiService;


        public ReportsController(ApiService apiService)
        {
            _apiService = apiService;

        }


        public IActionResult Index(string reportType = "")
        {
            ViewBag.PageTitle = reportType;
            return View(reportType);
        }


        [HttpGet]
        public async Task<IActionResult> GetTicketStateReport(string? StartDate, string EndDate)
        {
            try
            {

                var result = await _apiService.GetGenericResponse<string>($"api/Admin/GetTicketStateReport/{StartDate}/{EndDate}",true);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }
    }
}
