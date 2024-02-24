using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
using QLite.Data;
using QLite.Data.Dtos;
using Serilog;
using static QLite.Data.Models.Enums;

namespace QLiteDataApi.SignalR
{
    public class CommunicationHub : Hub
    {

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Debug("Communication Hub disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                string clientId = Context.GetHttpContext().Request.Query["clientId"];
                string clientType = Context.GetHttpContext().Request.Query["clientType"];
                string clientName = Context.GetHttpContext().Request.Query["clientName"];
             
                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientType) /*|| string.IsNullOrWhiteSpace(branchId)*/)
                {
                    throw new ArgumentNullException("clientId or clientType or branchId");
                }

                Log.Debug($"ws connection with clientId:{clientId} clientType:{clientType}");

                RegisterToGroups(clientId, clientType, clientName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "wshub-onConnected");
                throw;
            }

            return base.OnConnectedAsync();
        }

        private void RegisterToGroups(string clientId, string clientType, string clientName)
        {
            if (string.IsNullOrEmpty(clientId))
                return;

            clientId = clientId.ToLower().Split("_")[0];
            if (clientType == WebSocketClientType.Kiosk.ToString() || clientType == WebSocketClientType.Display.ToString())
            {
                var groupId = WebSocketClientType.Display.ToString() + "_" + clientId;
                Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                Log.Debug($"WSGROUP {clientType} /{clientName} {clientId}, -> {groupId} ");
            }
           
            else if (clientType == WebSocketClientType.User.ToString())
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "ALL_" );
                Groups.AddToGroupAsync(Context.ConnectionId, clientId);
            }
        }

        public async Task SendMessageToDesk(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToKiosk(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        

    }
}
