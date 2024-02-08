using Microsoft.AspNetCore.SignalR;
using Quavis.QorchLite.Common;

namespace Quavis.QorchLite.HwHost.SignalR
{
    public class HwHub : Hub
    {

        public HwHub()
        {
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            LoggerAdapter.Debug("Hub on disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            LoggerAdapter.Debug($"ws on connected");
            return base.OnConnectedAsync();
        }

    }
}
