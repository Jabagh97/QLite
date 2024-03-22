using KioskApp.Constants;
using KioskApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.DesignComponents;

namespace KioskApp.Controllers
{
    public class SegmentController : Controller
    {
        private readonly HttpService _httpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SegmentController(HttpService httpService, IHttpContextAccessor httpContextAccessor)
        {
            _httpService = httpService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string hwId)
        {
            try
            {
                var designData = await GetDesignData(hwId);
                var segments = await FetchSegmentsFromAPIAsync();

                // Store session data
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("StartTime", DateTime.Now.ToString());

                ViewBag.Segments = segments;
                return PartialView("~/Views/Home/Segments.cshtml", designData);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<DesPageData> GetDesignData(string hwId)
        {
            var step = "SegmentSelection";
            return await _httpService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }

        private async Task<List<Segment>> FetchSegmentsFromAPIAsync()
        {
            return await _httpService.GetGenericResponse<List<Segment>>(EndPoints.GetSegments);
        }
    }
}
