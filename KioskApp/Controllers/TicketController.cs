using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.Dtos;

namespace KioskApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index(TicketDto Ticket)
        {
            return PartialView("Views/Home/Ticket.cshtml");
        }


    }
}
