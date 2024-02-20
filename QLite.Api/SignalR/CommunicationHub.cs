using IdentityServer4.Models;
using Microsoft.AspNetCore.SignalR;
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
                string branchId = Context.GetHttpContext().Request.Query["branchId"];
                string branchName = Context.GetHttpContext().Request.Query["branchName"];
                string eventId = Context.GetHttpContext().Request.Query["eventId"];

                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientType) /*|| string.IsNullOrWhiteSpace(branchId)*/)
                {
                    throw new ArgumentNullException("clientId or clientType or branchId");
                }

                Log.Debug($"ws connection with clientId:{clientId} clientType:{clientType} branchId:{branchId}");

                RegisterToGroups(clientId, clientType, clientName, branchId, branchName, eventId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "wshub-onConnected");
                throw;
            }

            return base.OnConnectedAsync();
        }

        private void RegisterToGroups(string clientId, string clientType, string clientName, string branchId, string branchName, string eventId)
        {
            if (string.IsNullOrEmpty(clientId))
                return;

            clientId = clientId.ToLower().Split("_")[0];
            if (clientType == WebSocketClientType.Kiosk.ToString() || clientType == WebSocketClientType.Display.ToString())
            {
                var groupId = WebSocketClientType.Display.ToString() + "_" + clientId;
                Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                Log.Debug($"WSGROUP {clientType} {branchName}/{clientName} {clientId}, -> {groupId} ");
            }
            //else if (clientType == WebSocketClientType.MainDisplay.ToString())
            //{
            //    var mp = _kda.GetDisplay(clientId, true);
            //    LoggerAdapter.Debug($"WSGROUP {clientType} {branchName}/{clientName} {clientId}");
            //    foreach (var cp in mp.Children)
            //    {
            //        var groupId = WebSocketClientType.Display + "_" + cp.KioskId.ToString();
            //        Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            //        LoggerAdapter.Debug($"-WSGROUP child disp no: {cp.DisplayNo}, desk:{cp.DeskName} -> {groupId} ");
            //    }
            //}
            else if (clientType == WebSocketClientType.User.ToString())
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "ALL_" + branchId);
                Groups.AddToGroupAsync(Context.ConnectionId, clientId);
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                string[] events = eventId.Split(';');
                foreach (var ev in events)
                {
                    string groupId = ev;
                    if (!string.IsNullOrEmpty(branchId))
                        groupId += "_" + branchId;

                    Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                }
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
