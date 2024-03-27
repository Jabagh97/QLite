using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.Dtos;
using System.ComponentModel.DataAnnotations;
using KioskApp.Constants;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using QLite.DesignComponents;
using KioskApp.Services;
using QLite.Data.CommonContext;
using QLite.Data.Services;
using Serilog;
using KioskApp.Helpers;

namespace KioskApp.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApiService _httpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceController(ApiService httpService, IHttpContextAccessor httpContextAccessor)
        {
            _httpService = httpService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Index(Guid segmentOid, string hwId)
        {
            var viewModel = new ServicesAndDesignModel();
            try
            {
                viewModel.DesignData = await GetDesignData(hwId);
                viewModel.Services = await FetchServiceTypesFromAPIAsync(segmentOid);

                // Store necessary data in the session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Segment", segmentOid.ToString());


                Session.servicesAndDesignModel = viewModel;

                return PartialView("Views/Home/Services.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load services.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        private async Task<DesPageData> GetDesignDataTicket(string HwID)
        {
            var Step = "PagePrint";
            return await _httpService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{Step}/{HwID}");
        }
        private async Task<DesPageData> GetDesignData(string HwID)
        {
            var Step = "ServiceTypeSelection";
            return await _httpService.GetDesignResponse<DesPageData>($"api/Kiosk/GetDesignByKiosk/{Step}/{HwID}");
        }

        private async Task<List<ServiceTypeDto>> FetchServiceTypesFromAPIAsync(Guid segmentOid)
        {

            return await _httpService.GetGenericResponse<List<ServiceTypeDto>>($"api/Kiosk/GetServiceTypeList/{segmentOid}");

        }

       
        [HttpPost]
        public async Task<IActionResult> GetTicket([FromBody] TicketRequestDto ticketRequest)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var segmentIdString = session.GetString("Segment");
                if (Guid.TryParse(segmentIdString, out Guid segmentId))
                {
                    ticketRequest.SegmentId = segmentId;
                }

                var ticket = await _httpService.GetViewResponse<Ticket>(EndPoints.GetTicket, ticketRequest);
                if (ticket == null) return StatusCode(500, "Failed to retrieve ticket data");

                var hwId = CommonCtx.Config.GetValue<string>("KioskID"); 
                var designData = await GetDesignDataTicket(hwId);

                var model = new TicketAndDesPageDataViewModel
                {
                    Ticket = ticket,
                    DesPageData = designData
                };

                return PartialView("Views/Home/Ticket.cshtml", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching ticket.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> ChangeLanguage(Guid LangID)
        {

            try
            {
                CommonCtx.CurrentLanguage = LangID;

                return PartialView("~/Views/Home/Services.cshtml", Session.servicesAndDesignModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


    }
}
