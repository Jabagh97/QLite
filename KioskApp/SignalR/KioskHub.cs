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

        public async Task ReceiveMessage(string message)
        {
            // Log the received message
            Log.Debug($"Message received from Desk App: {message}");

            // You can perform any additional logic here, such as processing the message or broadcasting it to other clients
        }

        public async Task NotifyTicketState(string ticket)
        {
            // Log the received message
            Log.Debug($"Ticket received from server: {ticket}");

            // You can perform any additional logic here, such as processing the message or broadcasting it to other clients
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
