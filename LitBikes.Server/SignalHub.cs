using System;
using System.Collections.Generic;
using LitBikes.Game.Controller;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Model.Dtos;
using LitBikes.Model.Enums;
using LitBikes.Server;

namespace LitBikes.Game.Controller
{
    public class SignalHub : Hub
    {
        private readonly ClientEventReceiver _clientEventReceiver;
        private readonly ServerEventSender _serverEventSender;
        private readonly Dictionary<string, Guid> _playerConnections;

        public SignalHub(ClientEventReceiver clientEventReceiver, ServerEventSender serverEventSender)
        {
            _clientEventReceiver = clientEventReceiver;
            _serverEventSender = serverEventSender;
            _serverEventSender.Event += async (sender, args) => await SendEvent(args);
            _playerConnections = new Dictionary<string, Guid>();
        }

        public override Task OnConnectedAsync()
        {
            var newPlayerId = Guid.NewGuid();
            _playerConnections.Add(Context.ConnectionId, newPlayerId);
            return base.OnConnectedAsync();
        }

        private async Task SendEvent(ServerEventSenderArgs args)
        {
            await Clients.All.SendAsync(args.Event.ToString(), args.Payload);
        }

        #region ClientEvents

        public void Hello()
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Hello(playerId);
        }

        public void RequestJoinGame(ClientGameJoinDto dto)
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestJoinGame(playerId, dto);
        }

        public void KeepAlive()
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.KeepAlive(playerId);
        }

        public void RequestRespawn()
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestRespawn(playerId);
        }

        public void Update(ClientUpdateDto dto)
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Update(playerId, dto);
        }

        public void ChatMessage(ClientChatMessageDto dto)
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.ChatMessage(playerId, dto);
        }

        public void UsePowerup()
        {
            if (_playerConnections.TryGetValue(Context.ConnectionId, out var playerId))
                _clientEventReceiver.UsePowerup(playerId);
        }

        #endregion

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
