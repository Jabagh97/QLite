using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;
using Serilog;

namespace KioskApp.SignalR
{
    public class KioskHubContext : IHwHubContext
    {
        private IHubContext<KioskHub> _hubContext;

        public KioskHubContext(IHubContext<KioskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void HwEvent()
        {
            _hubContext.Clients.All.SendAsync("HwEvent", "Something Disconnected");

        }
      
    }
}
