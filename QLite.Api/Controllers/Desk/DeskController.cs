using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLite.CommonContext;
using QLite.Data;
using QLite.Data.Dtos;
using QLiteDataApi.Context;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Desk
{
    //[ApiController]
    public class DeskController : ControllerBase
    {
        private readonly IDeskService _deskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;
        private readonly IConfiguration _configuration;

        public DeskController(IDeskService deskService, IHubContext<CommunicationHub> communicationHubContext, IConfiguration configuration)
        {
            _deskService = deskService;
            _communicationHubContext = communicationHubContext;
            _configuration = configuration;
        }

        [HttpGet("api/Desk/GetWaitingTickets")]
        public async Task<IActionResult> GetWaitingTicketsAsync()
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Waiting);
            return Ok(jsonData);
        }

        [HttpGet("api/Desk/GetParkedTickets/{DeskID}")]
        public async Task<IActionResult> GetParkedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Park, DeskID);
            return Ok(jsonData);
        }

        [HttpGet("api/Desk/GetTransferedTickets/{DeskID}")]
        public async Task<IActionResult> GetTransferedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Waiting_T, DeskID);
            return Ok(jsonData);
        }

        [HttpGet("api/Desk/GetCompletedTickets/{DeskID}")]
        public async Task<IActionResult> GetCompletedTicketsAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetTicketsByStateAsync(TicketStateEnum.Final, DeskID);
            return Ok(jsonData);
        }

        [HttpGet("api/Desk/GetCurrentTicket/{DeskID}")]
        public async Task<IActionResult> GetCurrentTicketAsync(Guid DeskID)
        {
            var jsonData = await _deskService.GetMyCurrentServiceAsync(DeskID);
            return Ok(jsonData);
        }

        [HttpGet("api/Desk/CallTicket")]
        public async Task<IActionResult> CallTicketAsync(CallTicketDto callTicketDto)
        {
            var ticketState = await _deskService.CallTicketAsync(callTicketDto);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);
            string kioskId = _configuration.GetValue<string>("DisplayID");

            await Task.WhenAll(
                _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState),
                _communicationHubContext.Clients.Group("Display_" + kioskId).SendAsync("NotifyTicketState", serializedTicketState)
            );

            return Ok(serializedTicketState);
        }

        [HttpGet("api/Desk/EndTicket/{DeskID}")]
        public async Task<IActionResult> EndTicketAsync(Guid DeskID)
        {
            var ticketState = await _deskService.EndCurrentServiceAsync(DeskID);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(DeskID.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }

        [HttpGet("api/Desk/GetTicketDuration/{ticketid}")]
        public async Task<IActionResult> GetTicketDurationAsync(Guid ticketid)
        {
            int duration = await _deskService.GetTicketDurationAsync(ticketid);
            return Ok(duration);
        }

        [HttpPost("api/Desk/ParkTicket")]
        public async Task<IActionResult> ParkOperationAsync([FromBody][Required] ParkTicketDto parkTicket)
        {
            TicketState ticketState = await _deskService.ParkOperationAsync(parkTicket);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(parkTicket.DeskID.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }

        [HttpGet("api/Desk/GetDesk/{DeskID}")]
        public async Task<IActionResult> GetDeskAsync(Guid DeskID)
        {
            var desk = await _deskService.GetDeskAsync(DeskID);
            return Ok(desk);
        }

        [HttpGet("api/Desk/GetMacros/{DeskID}")]
        public async Task<IActionResult> GetMacrosAsync(Guid DeskID)
        {
            var macroSchedules = await _deskService.GetMacrosAsync(DeskID);
            return Ok(macroSchedules);
        }

        [HttpGet("api/Desk/GetDeskList")]
        public async Task<IActionResult> GetDeskListAsync()
        {
            var macroSchedules = await _deskService.GetDeskListAsync();
            return Ok(macroSchedules);
        }

        [HttpGet("api/Desk/GetCreatableServiceList/{DeskID}")]
        public async Task<IActionResult> GetCreatableServiceListAsync(Guid DeskID)
        {
            var creatableServices = await _deskService.GetCreatableServiceListAsync(DeskID);
            return Ok(creatableServices);
        }

        [HttpGet("api/Desk/GetTransferableServiceList/{DeskID}")]
        public async Task<IActionResult> GetTransferableServiceListAsync(Guid DeskID)
        {
            var transferableServices = await _deskService.GetTransferableServiceListAsync(DeskID);
            return Ok(transferableServices);
        }

        [HttpPost("api/Desk/TransferTicket")]
        public async Task<IActionResult> TransferOperationAsync([FromBody][Required] TransferTicketDto transferTicket)
        {
            TicketState ticketState = await _deskService.TransferOperationAsync(transferTicket);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            await _communicationHubContext.Clients.Group(transferTicket.TransferToDesk.ToString()).SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }

        [HttpGet("api/Desk/GetServiceList")]
        public async Task<IActionResult> GetServiceList()
        {
            var serviceTypes = await _deskService.GetServiceList();
            return Ok(serviceTypes);
        }


        [HttpGet("api/Desk/GetSegmentList")]
        public async Task<IActionResult> GetSegmentList()
        {
            var segments = await _deskService.GetSegmentList();
            return Ok(segments);
        }

        [HttpGet("api/Desk/SetBusyStatus/{DeskID}/{Status}")]
        public async Task<IActionResult> SetBusyStatus(Guid DeskID,DeskActivityStatus Status)
        {
            var status = await _deskService.SetBusyStatus(DeskID, Status);

            if (status !=null && Status != DeskActivityStatus.Open)
            {
                string kioskId = _configuration.GetValue<string>("DisplayID");

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


        [HttpGet("api/Desk/GetTicketStates/{TicketID}")]
        public async Task<IActionResult> GetTicketStates(Guid TicketID)
        {
            var jsonData = await _deskService.GetTicketStateListAsync(TicketID);
            return Ok(jsonData);
        }

    }


}
