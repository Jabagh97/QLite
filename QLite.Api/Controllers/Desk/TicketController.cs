using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLite.CommonContext;
using QLiteDataApi.Context;
using QLiteDataApi.Services;
using System.Drawing.Printing;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Desk
{
    public class TicketController : Controller
    {
        private readonly IDeskService _deskService;


        public TicketController(IDeskService deskService)
        {
            _deskService = deskService;
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
        [Route("api/Desk/CallTicket")]
        public IActionResult CallTicket(Guid DeskID, Guid ticketID, Guid user, Guid macroId)
        {
            var result = _deskService.CallTicket( DeskID,  ticketID,  user,  macroId);


            return Ok(result);
        }
    }

}
