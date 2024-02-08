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

        [HttpGet]
        public IActionResult GetWaitingTickets()
        {

            var waitingTickets = _dbContext.Tickets.ToList();

            var recordsTotal = waitingTickets.Count();
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = waitingTickets };

            return Ok(jsonData);
        }
    }
}
