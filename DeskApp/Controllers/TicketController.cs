using DeskApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Dtos;
using QLite.Data.Services;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace DeskApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketController(HttpClient httpClient, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        { 
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("APIBase"));

            _httpContextAccessor = httpContextAccessor;

        }

        public IActionResult Index()
        {
            return View("Views/Shared/layout/partials/_TicketContent.cshtml");
        }

        public async Task<string> GetUserIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CallTicketAsync(string TicketID)
        {
            try
            {

                var userId = await GetUserIdAsync();

                var response = await _httpClient.GetAsync($"api/Desk/CallTicket?DeskID=D07426D4-C92A-46E4-AD29-26F4CB1111B1&ticketID={TicketID}&user={userId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    TicketState ticketResponse = JsonConvert.DeserializeObject<TicketState>(responseData);


                    return Ok(ticketResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to call ticket");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        private async Task<IActionResult> GetTickets(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Desk/{endpoint}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    TicketResponse ticketResponse = JsonConvert.DeserializeObject<TicketResponse>(responseData);
                   
                    return Ok(ticketResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to get tickets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        [HttpGet]
        public Task<IActionResult> GetWaitingTickets() => GetTickets("GetWaitingTickets");

        [HttpGet]
        public Task<IActionResult> GetParkedTickets() => GetTickets("GetParkedTickets");

        [HttpGet]
        public Task<IActionResult> GetTransferedTickets() => GetTickets("GetTransferedTickets");

        [HttpGet]
        public Task<IActionResult> GetCompletedTickets() => GetTickets("GetCompletedTickets");

        [HttpGet]
        public async Task<IActionResult> GetCurrentTicket() 
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Desk/GetCurrentTicket?DeskID=D07426D4-C92A-46E4-AD29-26F4CB1111B1");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    TicketState ticketResponse = JsonConvert.DeserializeObject<TicketState>(responseData);


                     return PartialView("Components/MainPanel", ticketResponse);
                   // return ViewComponent("Components/MainPanel", ticketResponse);
                    // return Ok(ticketResponse);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to get tickets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> EndTicket() 
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Desk/EndTicket?DeskID=D07426D4-C92A-46E4-AD29-26F4CB1111B1");

                if (response.IsSuccessStatusCode)
                {

                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to get tickets");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ParkTicket([Required] ParkTicketDto parkTicket)

        {
            try
            {
                Guid deskId;

                Guid.TryParse("D07426D4-C92A-46E4-AD29-26F4CB1111B1", out deskId);
                parkTicket.DeskID = deskId;
                parkTicket.TicketNote = "TEST";

                var content = new StringContent(JsonConvert.SerializeObject(parkTicket), Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync($"api/Desk/ParkTicket", content);

                if (response.IsSuccessStatusCode)
                {
                  
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to call ticket");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }

        }

    }

}
