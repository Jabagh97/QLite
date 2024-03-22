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

namespace KioskApp.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HttpService _httpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceController(HttpService httpService, IHttpContextAccessor httpContextAccessor)
        {
            _httpService = httpService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> Index(Guid SegmentOid, string HwID)
        {
            try
            {

                DesPageData designData = await GetDesignData(HwID);


                ISession session = _httpContextAccessor.HttpContext.Session;

                session.SetString("Segment", SegmentOid.ToString());

                var services = await FetchServiceTypesFromAPIAsync(SegmentOid);


                ViewBag.Services = services;

                return PartialView("Views/Home/Services.cshtml", designData);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        private async Task<List<ServiceType>> FetchServiceTypesFromAPIAsync(Guid? segmentOid)
        {

            return await _httpService.GetGenericResponse<List<ServiceType>>($"{EndPoints.GetServiceTypeList}?segmentId={segmentOid}");

        }

        [HttpPost]
        public async Task<IActionResult> GetTicket([FromBody] TicketRequestDto ticketRequest)
        {
            try
            {
                ISession session = _httpContextAccessor.HttpContext.Session;

                string segmentIdString = session.GetString("Segment");

                if (Guid.TryParse(segmentIdString, out Guid segmentId))
                {
                    ticketRequest.SegmentId = segmentId;
                }

                var ticket = await _httpService.GetViewResponse<Ticket>(EndPoints.GetTicket, ticketRequest);

                if (ticket != null)
                {
                    string kioskHwId = CommonCtx.KioskHwId;

                    // Get the design data for the kiosk
                    DesPageData designData = await GetDesignDataTicket(kioskHwId);

                    // Create the view model
                    var model = new TicketAndDesPageDataViewModel
                    {
                        Ticket = ticket,
                        DesPageData = designData
                    };

                    // Return the partial view with the view model
                    return PartialView("Views/Home/Ticket.cshtml", model);
                }
                else
                {
                    return StatusCode(500, "Failed to retrieve ticket data");
                }
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
