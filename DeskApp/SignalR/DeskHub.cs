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

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task ReceiveMessage(string message)
        {
            // Log the received message
            Console.WriteLine($"Message received from Kiosk App: {message}");

        }
        public async Task NotifyTicketState(string state)
        {

            Console.WriteLine(state);
        }


        }
}
