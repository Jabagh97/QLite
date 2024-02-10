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

        public IActionResult Index()
           => View("Views/Shared/layout/partials/_TicketContent.cshtml");


        [HttpPost]
        public async Task<IActionResult> CallTicket()
        {
            try
            {
                //var response = await _apiService.PostAsync("api/Desk/CallTicket");

                //var x = await response.Content.ReadAsStringAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingTickets()
        {
            try
            {
                var response = await _apiService.GetAsync("api/Desk/GetWaitingTickets");

                var x = await response.Content.ReadAsStringAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetParkedTickets()
        {
            try
            {
                var response = await _apiService.GetAsync("api/Desk/GetParkedTickets");


                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetTransferedTickets()
        {
            try
            {
                var response = await _apiService.GetAsync("api/Desk/GetTransferedTickets");


                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }
    }
}
