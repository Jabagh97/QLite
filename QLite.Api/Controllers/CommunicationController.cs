using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QLiteDataApi.SignalR;

namespace QLiteDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunicationController : ControllerBase
    {
        private readonly IHubContext<CommunicationHub> _communicationHubContext;

        public CommunicationController(IHubContext<CommunicationHub> communicationHubContext)
        {
            _communicationHubContext = communicationHubContext;
        }

        [HttpPost("sendToDesk")]
        public async Task<IActionResult> SendMessageToDesk(string message)
        {
            await _communicationHubContext.Clients.Group("DeskClients").SendAsync("ReceiveMessage", message);
            return Ok();
        }

        [HttpPost("sendToKiosk")]
        public async Task<IActionResult> SendMessageToKiosk(string message)
        {
            await _communicationHubContext.Clients.Group("KioskClients").SendAsync("ReceiveMessage", message);
            return Ok();
        }
    }

}
