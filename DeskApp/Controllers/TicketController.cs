using DeskApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Services;
using System.Data;

namespace DeskApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TicketController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));
        }

        public IActionResult Index()
        {
            return View("Views/Shared/layout/partials/_TicketContent.cshtml");
        }

        [HttpPost]
        public IActionResult CallTicket()
        {
            // This method is not actually doing anything at the moment,
            // consider adding implementation as needed.
            return Ok();
        }

        private async Task<IActionResult> GetTickets(string endpoint, string ticketState)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Desk/{endpoint}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    TicketResponse ticketResponse = JsonConvert.DeserializeObject<TicketResponse>(responseData);

                    ViewBag.WaitingTickets = ticketResponse.recordsTotal;
                    ViewData["TicketState"] = ticketState;

                    return Ok(ticketResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to get {ticketState} tickets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        [HttpGet]
        public Task<IActionResult> GetWaitingTickets() => GetTickets("GetWaitingTickets", "Waiting Tickets");

        [HttpGet]
        public Task<IActionResult> GetParkedTickets() => GetTickets("GetParkedTickets", "Parked Tickets");

        [HttpGet]
        public Task<IActionResult> GetTransferedTickets() => GetTickets("GetTransferedTickets", "Transfered Tickets");
    }

}
