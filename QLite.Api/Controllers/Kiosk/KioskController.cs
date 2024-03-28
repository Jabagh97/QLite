using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using Serilog;
using System.Net.Sockets;

namespace QLiteDataApi.Controllers.Kiosk
{
    [ApiController]
    [Route("api/Kiosk")]
    public class KioskController : Controller
    {
        private readonly IKioskService _kioskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;

        public KioskController(IKioskService kioskService, IHubContext<CommunicationHub> communicationHubContext)
        {
            _communicationHubContext = communicationHubContext;
            _kioskService = kioskService;
        }

        [HttpPost("GetTicket")]
        public async Task<IActionResult> GetNewTicketAsync([FromBody] TicketRequestDto req)
        {
            try
            {
                Ticket newTicket = await _kioskService.GetNewTicketAsync(req);

                TicketState ticketState = newTicket.TicketStates.Last();

                //send ticketState to all clients registered
                string serializedTicketState = JsonConvert.SerializeObject(ticketState);

                await _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);

                string serializedTicket = JsonConvert.SerializeObject(newTicket);

                return Ok(serializedTicket);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetNewTicketAsync. Request: {@Request}", req);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("GetServiceTypeList/{segmentId}")]
        public async Task<IActionResult> GetServiceTypeList([FromRoute] Guid segmentId)
        {
            var serviceList = await _kioskService.GetServiceTypes(segmentId);
            return Ok(serviceList);
        }

        [HttpGet("GetSegments")]
        public async Task<IActionResult> GetSegments()
        {
            var segments = await _kioskService.GetSegments();
            return Ok(segments);
        }

        [HttpGet("GetKioskByHwID/{HwId}")]
        public async Task<IActionResult> GetKioskByHwID([FromRoute] string HwId)
        {
            var kiosk = await _kioskService.GetKioskByHwID(HwId);
            return Ok(kiosk);
        }

        [HttpGet("GetDesignByKiosk/{step}/{hwID}")]
        public async Task<IActionResult> GetDesignByKiosk([FromRoute] string step, [FromRoute] string hwID)
        {
            var design = await _kioskService.GetDesignByKiosk(step, hwID);

            if (design != null)
            {
                return Ok(design.DesignData);
            }

            return NotFound("Design not found.");
        }

        [HttpGet("GetResourceList")]
        public async Task<IActionResult> GetResourceList()
        {
            var resources = await _kioskService.GetResourceList();
            return Ok(resources);
        }

        [HttpGet("GetLanguageList")]
        public async Task<IActionResult> GetLanguageList()
        {
            var languages = await _kioskService.GetLanguageList();
            return Ok(languages);
        }

        [HttpGet("GetInServiceTickets/{KioskHwID}")]
        public async Task<IActionResult> GetInServiceTickets(string KioskHwID)
        {
            var services = await _kioskService.GetInServiceTickets(KioskHwID);
            return Ok(services);
        }
    }
}
