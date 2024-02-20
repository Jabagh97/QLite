using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;

namespace QLiteDataApi.SignalR
{
    public class CommunicationHubContext : IHwHubContext
    {
        private IHubContext<CommunicationHub> _hubContext;

        public CommunicationHubContext(IHubContext<CommunicationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void HwEvent(KioskHwStatusDto hwStatus)
        {
            _hubContext.Clients.All.SendAsync("hwevent", hwStatus);
        }

      
    }
}
