using Microsoft.AspNetCore.Mvc;
using QLite.Data;

namespace KioskApp.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index(Guid ServiceOid)
        {
            return PartialView("Views/Home/Ticket.cshtml");
        }
    }
}
