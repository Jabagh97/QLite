using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace AdminPortal.SignalR
{
    public class AdminHub : Hub
    {


        public override Task OnConnectedAsync()
        {
            Log.Debug($"Admin connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Debug("Admin disconnected");
            return base.OnDisconnectedAsync(exception);
        }



    }
}
