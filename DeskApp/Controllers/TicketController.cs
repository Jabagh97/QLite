using Microsoft.AspNetCore.Mvc;
using QLite.Data;
using QLite.Data.Services;

namespace DeskApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly IApiService _apiService;
        public TicketController(IApiService apiService)
        {
            _apiService = apiService;
        }



        [HttpGet]
        public async Task<IActionResult> GetWaitingTickets()
        {
            // Call service method to post data to API
            var result = await _apiService.GetAsync<List<Ticket>>("");

      
        }
    }
}
