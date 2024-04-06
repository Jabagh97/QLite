using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Context;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using Serilog;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers
{
    /// <summary>
    /// Handles desk-related operations including ticket management (waiting, parked, transferred, and completed tickets), 
    /// desk status updates, and fetching desk-related data such as service lists and macro schedules.
    /// </summary>

    [ApiController]
    [Route("api/Desk")]
    public class DeskController : ControllerBase
    {
        private readonly DeskService _deskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;

        public DeskController(DeskService deskService, IHubContext<CommunicationHub> communicationHubContext)
        {
            _deskService = deskService;
            _communicationHubContext = communicationHubContext;
        }

        /// <summary>
        /// Retrieves a list of tickets currently in a waiting state.
        /// </summary>
        /// <returns>A list of waiting tickets.</returns>
        [HttpGet("GetWaitingTickets")]
        public async Task<IActionResult> GetWaitingTicketsAsync()
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Waiting);
            return Ok(jsonData);
        }

        /// <summary>
        /// Retrieves a list of tickets that have been parked for a specified desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of parked tickets for the specified desk.</returns>
        /// 
        [HttpGet("GetParkedTickets/{DeskID}")]
        public async Task<IActionResult> GetParkedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Park, DeskID);
            return Ok(jsonData);
        }

        /// <summary>
        /// Retrieves a list of tickets that have been transferred to a specified desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of transferred tickets for the specified desk.</returns>
        [HttpGet("GetTransferedTickets/{DeskID}")]
        public async Task<IActionResult> GetTransferedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Waiting_T, DeskID);
            return Ok(jsonData);
        }


        /// <summary>
        /// Retrieves a list of tickets that have been completed at a specified desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of completed tickets for the specified desk.</returns>
        [HttpGet("GetCompletedTickets/{DeskID}")]
        public async Task<IActionResult> GetCompletedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Final, DeskID);
            return Ok(jsonData);
        }


        /// <summary>
        /// Retrieves the current ticket being served at the specified desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>The current ticket details if a ticket is being served; otherwise, null.</returns>
        [HttpGet("GetCurrentTicket/{DeskID}")]
        public async Task<IActionResult> GetCurrentTicketAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetMyCurrentServiceAsync(DeskID);
            return Ok(jsonData);
        }


        /// <summary>
        /// Calls a specific ticket to a desk, updating its state and notifying clients about the ticket state change.
        /// </summary>
        /// <param name="ticketID">The unique identifier of the ticket to call.</param>
        /// <param name="deskID">The unique identifier of the desk where the ticket is being called.</param>
        /// <param name="user">Optional parameter for the user ID making the call.</param>
        /// <param name="macroID">The unique identifier of the macro associated with the ticket call.</param>
        /// <returns>A success response if the ticket is successfully called; otherwise, an error message.</returns>
        [HttpGet("CallTicket")]
        public async Task<IActionResult> CallTicketAsync([FromQuery] Guid ticketID, [FromQuery] Guid deskID, [FromQuery] Guid? user, [FromQuery] Guid macroID)
        {
            if (deskID == Guid.Empty)
            {
                Log.Error("no Desk ID provided");
                return BadRequest("no Desk ID provided");
            }

            CallTicketDto callTicketDto = new CallTicketDto
            {
                TicketID = ticketID,
                DeskID = deskID,
                User = user ?? Guid.Empty,
                MacroID = macroID
            };
            var ticketState = await _deskService.CallTicketAsync(callTicketDto);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);
            string KioskWithUsbDisplay = ApiContext.Config.GetValue<string>("KioskWithUsbDisplay");
            if (ticketState == null)
            {
                await _communicationHubContext.Clients.Group(deskID.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            }
            else
            {
                string DisplayHwId = await _deskService.GetDisplayHwId(deskID);

                await Task.WhenAll(

               _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState),
               //To USB display Device
               _communicationHubContext.Clients.Group("Display_" + KioskWithUsbDisplay.ToLower()).SendAsync("NotifyTicketState"),
               //To Digital Signage Kiosk
               _communicationHubContext.Clients.Group("Display_" + DisplayHwId.ToLower()).SendAsync("NotifyDisplayKiosk")
                      );
            }


            return Ok();
        }

        /// <summary>
        /// Marks the current ticket at the specified desk as ended, updating its state and notifying clients.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk ending the ticket.</param>
        /// <returns>The ended ticket's state information.</returns>
        [HttpGet("EndTicket/{DeskID}")]
        public async Task<IActionResult> EndTicketAsync(Guid DeskID)
        {
            var ticketState = await _deskService.EndCurrentServiceAsync(DeskID);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(DeskID.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }


        /// <summary>
        /// Retrieves the duration a ticket has been active.
        /// </summary>
        /// <param name="ticketid">The unique identifier of the ticket.</param>
        /// <returns>The duration in minutes the ticket has been active.</returns>
        [HttpGet("GetTicketDuration/{ticketid}")]
        public async Task<IActionResult> GetTicketDurationAsync(Guid ticketid)
        {
            int duration = await _deskService.GetTicketDurationAsync(ticketid);
            return Ok(duration);
        }


        /// <summary>
        /// Parks a ticket, marking it for later follow-up and notifying relevant clients.
        /// </summary>
        /// <param name="parkTicket">The details of the ticket to be parked.</param>
        /// <returns>The parked ticket's state information.</returns>
        [HttpPost("ParkTicket")]
        public async Task<IActionResult> ParkOperationAsync([FromBody][Required] ParkTicketDto parkTicket)
        {
            TicketState ticketState = await _deskService.ParkOperationAsync(parkTicket);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(parkTicket.DeskID.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }


        /// <summary>
        /// Retrieves details for a specific desk by its ID.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>Details of the specified desk.</returns>
        [HttpGet("GetDesk/{DeskID}")]
        public async Task<IActionResult> GetDeskAsync(Guid DeskID)
        {
            var desk = await _deskService.GetDeskAsync(DeskID);
            return Ok(desk);
        }

        /// <summary>
        /// Retrieves a list of macros (preset actions or configurations) associated with a specific desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of macros for the specified desk.</returns>
        [HttpGet("GetMacros/{DeskID}")]
        public async Task<IActionResult> GetMacrosAsync(Guid DeskID)
        {
            var macroSchedules = await _deskService.GetMacrosAsync(DeskID);
            return Ok(macroSchedules);
        }

        /// <summary>
        /// Retrieves a list of all desks.
        /// </summary>
        /// <returns>A list of desks.</returns>
        [HttpGet("GetDeskList")]
        public async Task<IActionResult> GetDeskListAsync()
        {
            var macroSchedules = await _deskService.GetDeskListAsync();
            return Ok(macroSchedules);
        }

        /// <summary>
        /// Retrieves a list of services that can be initiated (created) at a specific desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of services creatable at the specified desk.</returns>
        [HttpGet("GetCreatableServiceList/{DeskID}")]
        public async Task<IActionResult> GetCreatableServiceListAsync(Guid DeskID)
        {
            var creatableServices = await _deskService.GetCreatableServiceListAsync(DeskID);
            return Ok(creatableServices);
        }

        /// <summary>
        /// Retrieves a list of services that can be transferred to another desk from the specified desk.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <returns>A list of transferable services from the specified desk.</returns>
        [HttpGet("GetTransferableServiceList/{DeskID}")]
        public async Task<IActionResult> GetTransferableServiceListAsync(Guid DeskID)
        {
            var transferableServices = await _deskService.GetTransferableServiceListAsync(DeskID);
            return Ok(transferableServices);
        }

        /// <summary>
        /// Transfers a ticket to another desk, updating its state and notifying relevant clients.
        /// </summary>
        /// <param name="transferTicket">The details of the ticket transfer operation.</param>
        /// <returns>The transferred ticket's state information.</returns>
        [HttpPost("TransferTicket")]
        public async Task<IActionResult> TransferOperationAsync([FromBody][Required] TransferTicketDto transferTicket)
        {
            TicketState ticketState = await _deskService.TransferOperationAsync(transferTicket);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(transferTicket.TransferToDesk.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }

        /// <summary>
        /// Retrieves a list of all available service types.
        /// </summary>
        /// <returns>A list of service types.</returns>
        [HttpGet("GetServiceList")]
        public async Task<IActionResult> GetServiceList()
        {
            var serviceTypes = await _deskService.GetServiceList();
            return Ok(serviceTypes);
        }


        /// <summary>
        /// Retrieves a list of all available segments.
        /// </summary>
        /// <returns>A list of segments.</returns>
        [HttpGet("GetSegmentList")]
        public async Task<IActionResult> GetSegmentList()
        {
            var segments = await _deskService.GetSegmentList();
            return Ok(segments);
        }

        /// <summary>
        /// Sets the busy status for a specified desk, potentially triggering notifications to clients.
        /// </summary>
        /// <param name="DeskID">The unique identifier of the desk.</param>
        /// <param name="Status">The new busy status to be set for the desk.</param>
        /// <returns>The new status of the desk.</returns>
        [HttpGet("SetBusyStatus/{DeskID}/{Status}")]
        public async Task<IActionResult> SetBusyStatus(Guid DeskID, DeskActivityStatus Status)
        {
            var status = await _deskService.SetBusyStatus(DeskID, Status);

            if (status != null && Status != DeskActivityStatus.Open)
            {
                string kioskId = ApiContext.Config.GetValue<string>("DisplayID");

                // Create the JSON object
                var notificationData = new
                {
                    DisplayNo = status,
                    TicketNo = "Busy",
                    SendToMain = true
                };

                // Serialize the JSON object to a string
                string jsonNotification = JsonConvert.SerializeObject(notificationData);

                // Send the JSON notification to clients in the group
                await _communicationHubContext.Clients.Group("Display_" + kioskId).SendAsync("NotifyTicketState", jsonNotification);
            }

            return Ok(status);
        }


        /// <summary>
        /// Retrieves a list of all states for a specific ticket.
        /// </summary>
        /// <param name="TicketID">The unique identifier of the ticket.</param>
        /// <returns>A list of states through which the ticket has progressed.</returns>
        [HttpGet("GetTicketStates/{TicketID}")]
        public async Task<IActionResult> GetTicketStates(Guid TicketID)
        {
            var jsonData = await _deskService.GetTicketStateListAsync(TicketID);
            return Ok(jsonData);
        }

    }


}
