using System;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Model.Dtos.FromClient;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public static class SendHub
    {
        public static void SendEvent(IHubClients clients, ServerEventSenderArgs args)
        {
            var clientProxy = clients.All;
            if (args.PlayerId != null)
            {
                ConnectionManager.GetConnectionId(args.PlayerId.Value, out var connectionId);
                clientProxy = clients.Client(connectionId);
            }
            new Task(async () => await clientProxy.SendAsync(args.Event.ToString(), args.Payload)).Start();
        }
    }

    public class SignalHub : Hub
    {
        private readonly IClientEventReceiver _clientEventReceiver;

        public SignalHub(IClientEventReceiver clientEventReceiver)
        {
            _clientEventReceiver = clientEventReceiver;
        }

        public override Task OnConnectedAsync()
        {
            ConnectionManager.OnConnected(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.OnDisconnected(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        #region ClientEvents

        public void Hello()
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Hello(playerId);
        }

        public void RequestJoinGame(ClientGameJoinDto dto)
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestJoinGame(playerId, dto);
        }

        public void KeepAlive()
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.KeepAlive(playerId);
        }

        public void RequestRespawn()
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestRespawn(playerId);
        }

        public void Update(ClientUpdateDto dto)
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Update(playerId, dto);
        }

        public void ChatMessage(ClientChatMessageDto dto)
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.ChatMessage(playerId, dto);
        }

        public void UsePowerup()
        {
            if (ConnectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.UsePowerup(playerId);
        }

        #endregion

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
