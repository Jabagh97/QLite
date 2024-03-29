using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using Serilog;
using System.Net.Sockets;

namespace QLiteDataApi.Controllers
{

    /// <summary>
    /// Controls the operations related to Kiosk functionalities such as ticket generation, 
    /// fetching services, segments, and design data.
    /// </summary>
    /// 
    [ApiController]
    [Route("api/Kiosk")]
    public class KioskController : Controller
    {
        private readonly KioskService _kioskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;

        public KioskController(KioskService kioskService, IHubContext<CommunicationHub> communicationHubContext)
        {
            _communicationHubContext = communicationHubContext;
            _kioskService = kioskService;
        }

        /// <summary>
        /// Generates a new ticket based on the provided request details and notifies all WebSocket clients (Desks) about the new ticket state.
        /// </summary>
        /// <param name="req">The ticket request details.</param>
        /// <returns>The newly generated ticket.</returns>
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


        /// <summary>
        /// Retrieves a list of service types associated with a given segment ID.
        /// </summary>
        /// <param name="segmentId">The unique identifier of the segment.</param>
        /// <returns>A list of service types for the specified segment.</returns>
        [HttpGet("GetServiceTypeList/{segmentId}")]
        public async Task<IActionResult> GetServiceTypeList([FromRoute] Guid segmentId)
        {
            var serviceList = await _kioskService.GetServiceTypes(segmentId);
            return Ok(serviceList);
        }


        /// <summary>
        /// Fetches all available segments.
        /// </summary>
        /// <returns>A list of all segments.</returns>
        [HttpGet("GetSegments")]
        public async Task<IActionResult> GetSegments()
        {
            var segments = await _kioskService.GetSegments();
            return Ok(segments);
        }

        /// <summary>
        /// Obtains details about a kiosk using its hardware ID.
        /// </summary>
        /// <param name="HwId">The hardware ID of the kiosk.</param>
        /// <returns>The kiosk details if found; otherwise, returns NotFound.</returns>

        [HttpGet("GetKioskByHwID/{HwId}")]
        public async Task<IActionResult> GetKioskByHwID([FromRoute] string HwId)
        {
            var kiosk = await _kioskService.GetKioskByHwID(HwId);
            return Ok(kiosk);
        }


        /// <summary>
        /// Retrieves design data for a specific step and hardware ID of a kiosk.
        /// </summary>
        /// <param name="step">The step in the kiosk operation for which design data is needed.</param>
        /// <param name="hwID">The hardware ID of the kiosk.</param>
        /// <returns>Design data for the given step and kiosk; otherwise, NotFound if not available.</returns>
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


        /// <summary>
        /// Provides a list of all resources available for the kiosk interface.
        /// </summary>
        /// <returns>A list of resources.</returns>
        [HttpGet("GetResourceList")]
        public async Task<IActionResult> GetResourceList()
        {
            var resources = await _kioskService.GetResourceList();
            return Ok(resources);
        }


        /// <summary>
        /// Gets a list of available languages for the kiosk interface.
        /// </summary>
        /// <returns>A list of languages.</returns>
        [HttpGet("GetLanguageList")]
        public async Task<IActionResult> GetLanguageList()
        {
            var languages = await _kioskService.GetLanguageList();
            return Ok(languages);
        }


        /// <summary>
        /// Retrieves a list of tickets that are currently being serviced for a given kiosk.
        /// </summary>
        /// <param name="KioskHwID">The hardware ID of the kiosk.</param>
        /// <returns>A list of in-service tickets.</returns>
        [HttpGet("GetInServiceTickets/{KioskHwID}")]
        public async Task<IActionResult> GetInServiceTickets(string KioskHwID)
        {
            var services = await _kioskService.GetInServiceTickets(KioskHwID);
            return Ok(services);
        }


        /// <summary>
        /// Fetches the default segment for Kiosks With service only type of Work flow.
        /// </summary>
        /// <returns>The default segment identifier.</returns>
        [HttpGet("GetDefaultSegment")]
        public async Task<IActionResult> GetDefaultSegment()
        {
            var defaultSegment = await _kioskService.GetDefaultSegment();
            return Ok(defaultSegment);
        }
    }
}
