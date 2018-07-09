using System;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Model.Dtos.FromClient;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public static class SendHub
    {
        public static async Task SendEvent(IHubClients clients, ServerEventSenderArgs args)
        {
            await clients.All.SendAsync(args.Event.ToString(), args.Payload);
        }
    }

    public class SignalHub : Hub
    {
        private readonly IClientEventReceiver _clientEventReceiver;
        private readonly ConnectionManager _connectionManager;

        public SignalHub(ConnectionManager connectionManager, IClientEventReceiver clientEventReceiver)
        {
            _clientEventReceiver = clientEventReceiver;
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            _connectionManager.OnConnected(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.OnDisconnected(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        #region ClientEvents

        public void Hello()
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Hello(playerId);
        }

        public void RequestJoinGame(ClientGameJoinDto dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestJoinGame(playerId, dto);
        }

        public void KeepAlive()
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.KeepAlive(playerId);
        }

        public void RequestRespawn()
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestRespawn(playerId);
        }

        public void Update(ClientUpdateDto dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Update(playerId, dto);
        }

        public void ChatMessage(ClientChatMessageDto dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.ChatMessage(playerId, dto);
        }

        public void UsePowerup()
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.UsePowerup(playerId);
        }

        #endregion

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
