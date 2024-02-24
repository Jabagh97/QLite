using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLite.CommonContext;
using QLite.Data;
using QLiteDataApi.Context;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using System.Drawing.Printing;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Desk
{
    public class TicketController : Controller
    {
        private readonly IDeskService _deskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;


        public TicketController(IDeskService deskService, IHubContext<CommunicationHub> communicationHubContext)
        {
            _deskService = deskService;
            _communicationHubContext = communicationHubContext;

        }

        private IActionResult GetTicketsByState(TicketStateEnum state)
        {
           var jsonData = _deskService.GetTicketsByState(state);

            return Ok(jsonData);
        }

        [HttpGet]
        [Route("api/Desk/GetWaitingTickets")]
        public IActionResult GetWaitingTickets()
        {
            return GetTicketsByState(TicketStateEnum.Waiting);
        }

        [HttpGet]
        [Route("api/Desk/GetParkedTickets")]
        public IActionResult GetParkedTickets()
        {
            return GetTicketsByState(TicketStateEnum.Park);
        }

        [HttpGet]
        [Route("api/Desk/GetTransferedTickets")]
        public IActionResult GetTransferedTickets()
        {
            return GetTicketsByState(TicketStateEnum.Waiting_T);
        }

        [HttpGet]
        [Route("api/Desk/GetCompletedTickets")]
        public IActionResult GetCompletedTickets()
        {
            return GetTicketsByState(TicketStateEnum.Final);
        }

        [HttpGet]
        [Route("api/Desk/GetCurrentTicket")]
        public IActionResult GetCurrentTicket(Guid DeskID)
        {
            var jsonData = _deskService.GetMyCurrentService(DeskID);

            return Ok(jsonData);
        }

        [HttpGet]
        [Route("api/Desk/CallTicket")]
        public IActionResult CallTicket(Guid DeskID, Guid ticketID, Guid user)
        {
            var ticketState = _deskService.CallTicket( DeskID,  ticketID,  user);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }


        [HttpGet]
        [Route("api/Desk/EndTicket")]
        public IActionResult EndTicket(Guid DeskID)
        {
            var ticketState = _deskService.EndCurrentService(DeskID);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }
    }

}
