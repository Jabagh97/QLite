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
using static QLite.Data.Models.Enums;

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
            var apiBase = _configuration.GetValue<string>("APIBase");
            if (string.IsNullOrEmpty(apiBase))
                throw new ArgumentException("APIBase configuration is missing or invalid.", nameof(apiBase));

            _httpClient.BaseAddress = new Uri(apiBase);
        }



        public IActionResult Index()
        {
            return View("Views/Shared/layout/partials/_TicketContent.cshtml");
        }
        [HttpPost]
        public Task<IActionResult> CallTicketAsync(Guid TicketID, Guid DeskID, Guid MacroID) =>
            HandleHttpRequest(async () => await _httpClient.GetAsync($"api/Desk/CallTicket?DeskID={DeskID}&ticketID={TicketID}&user={Guid.Empty}&macroID={MacroID}"));


        [HttpGet]
        public Task<IActionResult> EndTicket(Guid DeskID) =>
            HandleHttpRequest(async () => await _httpClient.GetAsync($"api/Desk/EndTicket/{DeskID}"));

        [HttpGet]
        public Task<IActionResult> GetDesk(Guid DeskID) =>
            GetJsonResponse<Desk>($"api/Desk/GetDesk/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetMacros(Guid DeskID) =>
            GetJsonResponse<List<DeskMacroSchedule>>($"api/Desk/GetMacros/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetDeskList() =>
            GetJsonResponse<List<Desk>>($"api/Desk/GetDeskList");

        [HttpGet]
        public Task<IActionResult> GetTransferableServiceList(Guid DeskID) =>
            GetJsonResponse<List<DeskTransferableService>>($"api/Desk/GetTransferableServiceList/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetCreatableServicesList(Guid DeskID) =>
            GetJsonResponse<List<DeskCreatableService>>($"api/Desk/GetCreatableServiceList/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetWaitingTickets() =>
            GetJsonResponse<TicketResponse>("api/Desk/GetWaitingTickets");

        [HttpGet]
        public Task<IActionResult> GetParkedTickets([Required] Guid DeskID) =>
            GetJsonResponse<TicketResponse>($"api/Desk/GetParkedTickets/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetTransferedTickets([Required] Guid DeskID) =>
            GetJsonResponse<TicketResponse>($"api/Desk/GetTransferedTickets/{DeskID}");

        [HttpGet]
        public Task<IActionResult> GetCompletedTickets([Required] Guid DeskID) =>
            GetJsonResponse<TicketResponse>($"api/Desk/GetCompletedTickets/{DeskID}");

        [HttpPost]
        public Task<IActionResult> ParkTicket([Required] ParkTicketDto parkTicket) =>
            PostJsonRequest($"api/Desk/ParkTicket", parkTicket);

        [HttpPost]
        public Task<IActionResult> TransferTicket([Required] TransferTicketDto transferTicket) =>
            PostJsonRequest($"api/Desk/TransferTicket", transferTicket);

        [HttpGet]
        public Task<IActionResult> GetCurrentTicket(Guid DeskID) =>
            GetViewResponse<TicketState>($"api/Desk/GetCurrentTicket/{DeskID}", "Components/MainPanel");


        [HttpGet]
        public Task<IActionResult> GetSegmentList() =>
            GetJsonResponse<List<Segment>>($"api/Desk/GetSegmentList");

        [HttpPost]
        public Task<IActionResult> CreateTicket([Required] TicketRequestDto ticketRequest) =>
           PostJsonRequest($"api/Kiosk/GetTicket", ticketRequest);


        [HttpGet]
        public Task<IActionResult> SetBusyStatus(Guid DeskID,DeskActivityStatus Status) =>
            GetJsonResponse<DeskActivityStatus>($"api/Desk/SetBusyStatus/{DeskID}/{Status}");


        [HttpGet]
        public Task<IActionResult> GetTicketStates([Required] Guid TicketID) =>
           GetJsonResponse<TicketStateResponse>($"api/Desk/GetTicketStates/{TicketID}");

        #region Helpers
        private async Task<IActionResult> GetViewResponse<T>(string endpoint, string view)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    T deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                    return PartialView(view, deserializedData);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to retrieve data");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }
        private async Task<IActionResult> HandleHttpRequest(Func<Task<HttpResponseMessage>> request)
        {
            try
            {
                var response = await request();

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"Failed to complete the operation");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        private async Task<IActionResult> GetJsonResponse<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                T deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                return Ok(deserializedData);
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Failed to retrieve data");
            }
        }


        private async Task<IActionResult> PostJsonRequest(string endpoint, object data)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);

            return await HandleHttpRequest(async () => response);
        }
        #endregion

    }
}
