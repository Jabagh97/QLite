using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace KioskApp.SignalR
{
    public class KioskHub : Hub
    {


        public override Task OnConnectedAsync()
        {
            Log.Debug($"Kiosk connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Debug("Kiosk disconnected");
            return base.OnDisconnectedAsync(exception);
        }

    }
}
