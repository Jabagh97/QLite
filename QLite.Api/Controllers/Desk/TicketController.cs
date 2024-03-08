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
    public class TicketController : Controller
    {
        private readonly IDeskService _deskService;
        private readonly IHubContext<CommunicationHub> _communicationHubContext;
        private readonly IConfiguration _configuration;


        public TicketController(IDeskService deskService, IHubContext<CommunicationHub> communicationHubContext, IConfiguration configuration)
        {
            _deskService = deskService;
            _communicationHubContext = communicationHubContext;
            _configuration = configuration;
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
        public IActionResult CallTicket(Guid DeskID, Guid ticketID, Guid user, Guid MacroID)
        {
            var ticketState = _deskService.CallTicket( DeskID,  ticketID,  user,  MacroID);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);
            string kioskId = _configuration.GetValue<string>("DisplayID");

            _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);
            _communicationHubContext.Clients.Group("Display_"+ kioskId).SendAsync("NotifyTicketState", serializedTicketState);


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


        [HttpGet]
        [Route("api/GetTicketDuration/{ticketid}")]
        public ActionResult GetTicketDuration([FromRoute] Guid ticketid)
        {
            int duration = _deskService.GetTicketDuration(ticketid);
            return Ok(duration);
        }

        [HttpPost]
        [Route("api/Desk/ParkTicket")]
        public ActionResult ParkOperation([FromBody][Required] ParkTicketDto parkTicket)
        {
            TicketState ticketState = _deskService.ParkOperation(parkTicket);

            string serializedTicketState = JsonConvert.SerializeObject(ticketState);

            _communicationHubContext.Clients.Group("ALL_").SendAsync("NotifyTicketState", serializedTicketState);

            return Ok(serializedTicketState);
        }


        [HttpGet]
        [Route("api/Desk/GetDesk/{DeskID}")]
        public ActionResult GetDesk([FromRoute] Guid DeskID)
        {
            var desk = _deskService.GetDesk(DeskID);
            return Ok(desk);
        }

        [HttpGet]
        [Route("api/Desk/GetMacros/{DeskID}")]
        public ActionResult GetMacros([FromRoute] Guid DeskID)
        {
            var macroSchedules = _deskService.GetMacros(DeskID);
            return Ok(macroSchedules);
        }
    }

}
