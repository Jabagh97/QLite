using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Context;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using Serilog;
using System.Net.Sockets;
using System.Text;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Kiosk
{
    public class KioskController : Controller
    {

        private readonly IKioskService _KioskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;


        public KioskController(IKioskService KioskService, IHubContext<CommunicationHub> communicationHubContext)
        {
            _communicationHubContext = communicationHubContext;
            _KioskService = KioskService;
        }

        [HttpPost]
        [Route("api/Kiosk/GetTicket")]
        public async Task<IActionResult> GetNewTicketAsync([FromBody] TicketRequestDto req)
        {
            try
            {
                Ticket newTicket = await _KioskService.GetNewTicketAsync(req);

                TicketState ticketState = newTicket.TicketStates.Last();

                //send ticketState to all clients registered
                string serializedTicketState = JsonConvert.SerializeObject(ticketState);

                await _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);

                string serializedTicket = JsonConvert.SerializeObject(newTicket);

                return Ok(serializedTicket);
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate response
                Log.Error("Kiosk Error: Error Creating Ticket");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("api/Kiosk/GetServiceTypeList")]
        public async Task<IActionResult> GetServiceTypeList(Guid segmentId)
        {
            var ServiceList = await _KioskService.GetServiceTypes(segmentId);
            string serializedList = JsonConvert.SerializeObject(ServiceList);

            return Ok(serializedList);
        }

        [HttpGet]
        [Route("api/Kiosk/GetSegments")]
        public IActionResult GetSegments()
        {
            var segments = _KioskService.GetSegments();

            return Ok(segments);

        }

        [HttpGet]
        [Route("api/Kiosk/GetKioskByHwID")]
        public IActionResult GetKioskByHwID(string HwId)
        {
            var kiosk = _KioskService.GetKioskByHwID(HwId);

            if (kiosk.Count() == 0 || kiosk.Count() > 1) // Check if kiosk is null or empty
            {
                Log.Error("Kiosk Error: Kiosk not found for the provided Hardware ID.");

                return NotFound("Kiosk not found for the provided Hardware ID.");
            }

            return Ok(kiosk);
        }

    }
}
