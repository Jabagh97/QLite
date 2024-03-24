﻿using KioskApp.Constants;
using KioskApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Services;
using QLite.DesignComponents;
using Serilog;

namespace KioskApp.Controllers
{
    public class SegmentController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SegmentController(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string hwId)
        {
            var viewModel = new SegmentsAndDesignModel();
            try
            {
                viewModel.DesignData = await GetDesignData(hwId);
                viewModel.Segments = await FetchSegmentsFromAPIAsync();

                // Consider if there's a more efficient way to handle this or if it's necessary
                _httpContextAccessor.HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                // Logging the exception
                Log.Error(ex, "Failed to load segments.");

                // In a global error handler, you'd handle this more gracefully
                return StatusCode(500, "Internal server error. Please try again later.");
            }

            return PartialView("~/Views/Home/Segments.cshtml", viewModel);
        }

        private async Task<DesPageData> GetDesignData(string hwId)
        {
            var step = "SegmentSelection";
            return await _apiService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }

        private async Task<List<Segment>> FetchSegmentsFromAPIAsync()
        {
            return await _apiService.GetGenericResponse<List<Segment>>(EndPoints.GetSegments);
        }
    }

   

}
