﻿using KioskApp.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;

namespace KioskApp.Controllers
{
    public class SegmentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SegmentController(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ISession session = _httpContextAccessor.HttpContext.Session;

                session.SetString("StartTime", DateTime.Now.ToString());

                var segments = await FetchSegmentsFromAPIAsync();

                ViewBag.Segments = segments;

                return PartialView("Views/Home/Segments.cshtml");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<List<Segment>> FetchSegmentsFromAPIAsync()
        {
            var response = await _httpClient.GetAsync(EndPoints.GetSegments);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var segments = JsonConvert.DeserializeObject<List<Segment>>(json);

            return segments;
        }
    }
}
