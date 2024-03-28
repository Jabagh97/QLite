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
using static QLite.Data.Models.Enums;

namespace KioskApp.Controllers
{
    public class KioskController : Controller
    {
        private readonly ApiService _apiService;
        private readonly HwManager _hwman;

        public KioskController(ApiService httpService, HwManager hwman)
        {
            _hwman = hwman;
            _apiService = httpService;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await InitHomepage();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading the Kiosk page.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }

        public async Task<IActionResult> GetSegmentView(string hwId)
        {
            try
            {
                var viewModel = new SegmentsAndDesignModel
                {
                    DesignData = await GetDesignData(hwId, Step.SegmentSelection.ToString()),
                    Segments = await GetSegmentList()
                };

                Session.segmentsAndDesignModel = viewModel;

                return PartialView("~/Views/Kiosk/Segments.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load segments view.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        public async Task<IActionResult> GetServiceView(Guid segmentOid, string hwId)
        {

            try
            {
                var viewModel = new ServicesAndDesignModel
                {
                    DesignData = await GetDesignData(hwId, Step.ServiceTypeSelection.ToString()),
                    Services = await GetServiceList(segmentOid)
                };

                Session.selectedSegment = segmentOid;

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
                ticketRequest.SegmentId = Session.selectedSegment;

                var ticket = await _apiService.PostGenericRequest<Ticket>("api/Kiosk/GetTicket", ticketRequest);

                if (ticket == null) return StatusCode(500, "Failed to retrieve ticket data");

                var hwId = CommonCtx.Config.GetValue<string>("KioskID");

                var designData = await GetDesignData(hwId, Step.PagePrint.ToString());

                var model = new TicketAndDesPageDataViewModel
                {
                    Ticket = ticket,
                    DesPageData = designData
                };

                Session.ticketAndDesPageData = model;


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
        private async Task<List<SegmentDto>> GetSegmentList()
        {
            return await _apiService.GetGenericResponse<List<SegmentDto>>("api/Kiosk/GetSegments");
        }
        private async Task<List<ServiceTypeDto>> GetServiceList(Guid segmentOid)
        {

            return await _apiService.GetGenericResponse<List<ServiceTypeDto>>($"api/Kiosk/GetServiceTypeList/{segmentOid}");

        }

        [HttpPost]
        public IActionResult ChangeLanguage(Guid LangID, string step)
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

        private async Task<HomeAndDesPageDataViewModel> InitHomepage()
        {
            var viewModel = new HomeAndDesPageDataViewModel
            {
                DesPageData = await GetDesignData(CommonCtx.KioskHwId, Step.WelcomePage.ToString()),
                KioskHwId = CommonCtx.KioskHwId
            };

            CommonCtx.Languages = await GetLanguageList();
            CommonCtx.Resources = await GetResourceList();
            CommonCtx.CurrentLanguage = CommonCtx.Languages.FirstOrDefault()?.Oid ?? Guid.Empty;

            Session.homeAndDesPageData = viewModel;
            return viewModel;
        }


        #region Hardware Requests

        [HttpPost]
        public IActionResult PrintTicket([FromBody] TicketViewModel viewModel)
        {
            try
            {
                var copies = Session.ticketAndDesPageData.Ticket.CopyNumber;

                if (copies.HasValue)
                {
                    // Execute the loop the number of times specified by copies.
                    for (int i = 0; i < copies.Value; i++)
                    {
                        _hwman.Print(viewModel.Html);
                    }
                }


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
        #endregion

    }
}