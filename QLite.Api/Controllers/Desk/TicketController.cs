using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLite.CommonContext;
using QLiteDataApi.Context;
using System.Drawing.Printing;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.Controllers.Desk
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TicketController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IActionResult GetTicketsByState(TicketStateEnum state)
        {
            var query = from t in _dbContext.Tickets
                        join ts in _dbContext.TicketStates on t.Oid equals ts.Ticket
                        join d in _dbContext.Desks on ts.Desk equals d.Oid into deskJoin
                        from desk in deskJoin.DefaultIfEmpty()
                        join tp in _dbContext.TicketPools on t.TicketPool equals tp.Oid into poolJoin
                        from pool in poolJoin.DefaultIfEmpty()
                        where t.ModifiedDate >= DateTime.UtcNow.AddHours(-8) &&
                              t.CurrentState == (int)state &&
                              t.Branch == UserContext.BranchId
                        select new
                        {
                            Ticket = t,
                            ServiceCode = pool.ServiceCode,
                            TicketState = ts,
                            DeskName = desk.Name
                        };

            var waitingTickets = query.ToList();

            var recordsTotal = waitingTickets.Count;
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = waitingTickets };

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
    }

}
