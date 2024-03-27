using KioskApp.Constants;
using KioskApp.Helpers;
using KioskApp.Models;
using KioskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;
using QLite.Data.Services;
using QLite.DesignComponents;
using Quavis.QorchLite.Hwlib;
using Quavis.QorchLite.Hwlib.Display;
using Serilog;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;

namespace KioskApp.Controllers
{
    public class KioskController : Controller
    {
        private readonly ApiService _apiService;
        private readonly HwManager _hwman;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KioskController(ApiService httpService, HwManager hwman, IHttpContextAccessor httpContextAccessor)
        {
            _hwman = hwman;
            _apiService = httpService;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeAndDesPageDataViewModel();
            try
            {
                CommonCtx.Languages = await GetLanguageList();
                CommonCtx.Resources = await GetResourceList();

                // Check if CurrentLanguage is empty
                if (CommonCtx.CurrentLanguage == Guid.Empty)
                {
                    var firstLanguage = CommonCtx.Languages.FirstOrDefault();
                    // Ensure there's at least one language available
                    if (firstLanguage != null)
                    {
                        CommonCtx.CurrentLanguage = firstLanguage.Oid;
                    }
                }

                string _kioskHwId = CommonCtx.KioskHwId;
                DesPageData designData = await GetDesignData(_kioskHwId, "WelcomePage");

                viewModel.DesPageData = designData;
                viewModel.KioskHwId = _kioskHwId;


                Session.homeAndDesPageData = viewModel;



                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetSegmentView(string hwId)
        {
            var viewModel = new SegmentsAndDesignModel();
            try
            {
                viewModel.DesignData = await GetDesignData(hwId, "SegmentSelection");
                viewModel.Segments = await FetchSegmentsFromAPIAsync();

                _httpContextAccessor.HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());


                Session.segmentsAndDesignModel = viewModel;
            }
            catch (Exception ex)
            {
                // Logging the exception
                Log.Error(ex, "Failed to load segments.");

                // In a global error handler, you'd handle this more gracefully
                return StatusCode(500, "Internal server error. Please try again later.");
            }

            return PartialView("~/Views/Kiosk/Segments.cshtml", viewModel);
        }

        public async Task<IActionResult> GetServiceView(Guid segmentOid, string hwId)
        {
            var viewModel = new ServicesAndDesignModel();
            try
            {
                viewModel.DesignData = await GetDesignData(hwId, "ServiceTypeSelection");
                viewModel.Services = await FetchServiceTypesFromAPIAsync(segmentOid);

                // Store necessary data in the session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Segment", segmentOid.ToString());


                Session.servicesAndDesignModel = viewModel;

                return PartialView("Views/Kiosk/Services.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load services.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetTicketView([FromBody] TicketRequestDto ticketRequest)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var segmentIdString = session.GetString("Segment");
                if (Guid.TryParse(segmentIdString, out Guid segmentId))
                {
                    ticketRequest.SegmentId = segmentId;
                }

                var ticket = await _apiService.GetViewResponse<Ticket>(EndPoints.GetTicket, ticketRequest);
                if (ticket == null) return StatusCode(500, "Failed to retrieve ticket data");

                var hwId = CommonCtx.Config.GetValue<string>("KioskID");
                var designData = await GetDesignData(hwId, "PagePrint");

                var model = new TicketAndDesPageDataViewModel
                {
                    Ticket = ticket,
                    DesPageData = designData
                };

                return PartialView("Views/Kiosk/Ticket.cshtml", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching ticket.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<List<Resource>> GetResourceList()
        {
            return await _apiService.GetDesignResponse<List<Resource>>($"api/Kiosk/GetResourceList");

        }

        private async Task<List<Language>> GetLanguageList()
        {
            return await _apiService.GetDesignResponse<List<Language>>($"api/Kiosk/GetLanguageList");
        }


        private async Task<DesPageData> GetDesignData(string hwId, string step)
        {
            return await _apiService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{step}/{hwId}");
        }
        private async Task<List<SegmentDto>> FetchSegmentsFromAPIAsync()
        {
            return await _apiService.GetGenericResponse<List<SegmentDto>>("api/Kiosk/GetSegments");
        }
        private async Task<List<ServiceTypeDto>> FetchServiceTypesFromAPIAsync(Guid segmentOid)
        {

            return await _apiService.GetGenericResponse<List<ServiceTypeDto>>($"api/Kiosk/GetServiceTypeList/{segmentOid}");

        }

        [HttpPost]

        public async Task<IActionResult> ChangeLanguage(Guid LangID, string step )
        {
            try
            {
                CommonCtx.CurrentLanguage = LangID;

                switch (step)
                {
                    case "WelcomePage":
                        return PartialView("~/Views/Kiosk/Index.cshtml", Session.homeAndDesPageData);

                    case "SegmentSelection":
                        return PartialView("~/Views/Kiosk/Segments.cshtml", Session.segmentsAndDesignModel);

                    case "ServiceTypeSelection":
                        return PartialView("~/Views/Kiosk/Services.cshtml", Session.servicesAndDesignModel);

                    default:
                        return NotFound("The specified step is not recognized.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost]
        public IActionResult PrintTicket([FromBody] TicketViewModel viewModel)
        {
            try
            {
                _hwman.Print(viewModel.Html);

                return Ok("Print successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Printing failed: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DisplayTicket([FromBody] QueNumData queNumData)
        {
            try
            {
                _hwman.Display(queNumData);

                return Ok("Print successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Printing failed: {ex.Message}");
            }
        }
        public IActionResult CheckKiosk()
        {
            return Ok(_hwman.GetKioskHwStatus());
        }


    }
}