using Microsoft.AspNetCore.SignalR;
using QLite.Dto;
using Quavis.QorchLite.HwHost.SignalR;
using Quavis.QorchLite.Hwlib;

namespace Quavis.QorchLite.Hwhost.SignalR
{
    public class HwHostHubContext: IHwHubContext
    {
        private IHubContext<HwHub> _hubContext;

        public HwHostHubContext(IHubContext<HwHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void HwEvent(KioskHwStatusDto hwStatus)
        {
            _hubContext.Clients.All.SendAsync("hwevent", hwStatus);

        }
    }


}
