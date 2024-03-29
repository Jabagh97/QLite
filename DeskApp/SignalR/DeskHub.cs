using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using QLite.Data;
using Serilog;

namespace DeskApp.SignalR
{
    public class DeskHub : Hub
    {


        public override Task OnConnectedAsync()
        {
            Log.Debug($"Desk connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Debug("Desk disconnected");
            return base.OnDisconnectedAsync(exception);
        }



    }
}
