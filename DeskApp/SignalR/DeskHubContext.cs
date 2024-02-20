using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using QLite.Data.CommonContext;
using QLite.Data.Dtos;

namespace DeskApp.SignalR
{
    public class DeskHubContext : IHwHubContext
    {
        private IHubContext<DeskHub> _hubContext;

        public DeskHubContext(IHubContext<DeskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void HwEvent(KioskHwStatusDto hwStatus)
        {
            throw new NotImplementedException();
        }

      
    }
}
