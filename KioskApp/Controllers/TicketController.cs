using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;

namespace KioskApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketController(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;

        }
        public IActionResult Index(string ticketJson) 
        {
            try
            {
                Ticket ticket = JsonConvert.DeserializeObject<Ticket>(ticketJson);

                ISession session = _httpContextAccessor.HttpContext.Session;
                string startTimeString = session.GetString("StartTime");
                string segmentIdString = session.GetString("Segment");

                // Do whatever you need to do with the ticket object
                // For example, pass it to a view
                return PartialView("Views/Home/Ticket.cshtml", ticket);
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                return StatusCode(400, $"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
