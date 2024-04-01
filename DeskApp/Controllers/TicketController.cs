using DeskApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly ApiService _apiService;


        public TicketController(ApiService apiService)
        {
            _apiService = apiService;

        }



        public IActionResult Index()
        {
            return View("Views/Shared/layout/partials/_TicketContent.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> CallTicketAsync(Guid TicketID, Guid DeskID, Guid MacroID)
        {
            var result = await _apiService.GetGenericResponse<string>($"api/Desk/CallTicket?DeskID={DeskID}&ticketID={TicketID}&user={Guid.Empty}&macroID={MacroID}", true);
            return Ok(result);

        }


        [HttpGet]
        public async Task<IActionResult> EndTicket(Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<string>($"api/Desk/EndTicket/{DeskID}", true);
            return Ok(result);

        }

        [HttpGet]
        public async Task<IActionResult> GetDesk(Guid DeskID)
        {

            var result = await _apiService.GetGenericResponse<Desk>($"api/Desk/GetDesk/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMacros(Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<List<DeskMacroSchedule>>($"api/Desk/GetMacros/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeskList()
        {
            var result = await _apiService.GetGenericResponse<List<Desk>>($"api/Desk/GetDeskList");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransferableServiceList(Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<List<DeskTransferableService>>($"api/Desk/GetTransferableServiceList/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCreatableServicesList(Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<List<DeskCreatableService>>($"api/Desk/GetCreatableServiceList/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingTickets()
        {
            var result = await _apiService.GetGenericResponse<TicketResponse>("api/Desk/GetWaitingTickets");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetParkedTickets([Required] Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<TicketResponse>($"api/Desk/GetParkedTickets/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransferedTickets([Required] Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<TicketResponse>($"api/Desk/GetTransferedTickets/{DeskID}");

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompletedTickets([Required] Guid DeskID)
        {
            var result = await _apiService.GetGenericResponse<TicketResponse>($"api/Desk/GetCompletedTickets/{DeskID}");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ParkTicket([Required] ParkTicketDto parkTicket)
        {
            var response = await _apiService.PostGenericRequest<bool>($"api/Desk/ParkTicket", parkTicket, true);

            if (response)
            {
                return Ok();
            }
            return StatusCode(500, $"Failed to complete the operation");

        }

        [HttpPost]
        public async Task<IActionResult> TransferTicket([Required] TransferTicketDto transferTicket)
        {
            var response = await _apiService.PostGenericRequest<bool>($"api/Desk/TransferTicket", transferTicket, true);

            if (response)
            {
                return Ok();
            }
            return StatusCode(500, $"Failed to complete the operation");

        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([Required] TicketRequestDto ticketRequest)
        {
            var response = await _apiService.PostGenericRequest<bool>($"api/Kiosk/GetTicket", ticketRequest, true);

            if (response)
            {
                return Ok();
            }
            return StatusCode(500, $"Failed to complete the operation");

        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentTicket(Guid DeskID)
        {
            return await GetViewResponse<TicketState>($"api/Desk/GetCurrentTicket/{DeskID}", "Components/MainPanel");
        }


        [HttpGet]
        public async Task<IActionResult> GetSegmentList()
        {
            var result = await _apiService.GetGenericResponse<List<Segment>>($"api/Desk/GetSegmentList");

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> SetBusyStatus(Guid DeskID, DeskActivityStatus Status)
        {
            var result = await _apiService.GetGenericResponse<DeskActivityStatus>($"api/Desk/SetBusyStatus/{DeskID}/{Status}");

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetTicketStates([Required] Guid TicketID)
        {
            var result = await _apiService.GetGenericResponse<TicketStateResponse>($"api/Desk/GetTicketStates/{TicketID}");

            return Ok(result);
        }

        #region Helpers
        private async Task<IActionResult> GetViewResponse<T>(string endpoint, string view)
        {
            try
            {
                var result = await _apiService.GetGenericResponse<T>(endpoint);

                return PartialView(view, result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }





        #endregion

    }
}
