using System;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Model.Dtos.FromClient.Short;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public static class SendHub
    {
        private static DateTime lastEvent;
        public static void SendEvent(IHubClients clients, ServerEventSenderArgs args)
        {
            var startTime = DateTime.Now;
            Console.WriteLine($"SignalHub received game event at {startTime}");
            var date = DateTime.UtcNow;
            var dur = date - lastEvent;
            Console.WriteLine($"Time since last SignalR event broadcast: {dur.TotalMilliseconds}ms");
            new Task(async () => await clients.All.SendAsync(args.Event.ToString(), args.Payload)).Start();
            lastEvent = date;
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

        public void RequestJoinGame(ClientGameJoinDtoShort dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.RequestJoinGame(playerId, (ClientGameJoinDto) dto.MapToFullDto());
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

        public void Update(ClientUpdateDtoShort dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.Update(playerId, (ClientUpdateDto) dto.MapToFullDto());
        }

        public void ChatMessage(ClientChatMessageDtoShort dto)
        {
            if (_connectionManager.PlayerExists(Context.ConnectionId, out var playerId))
                _clientEventReceiver.ChatMessage(playerId, (ClientChatMessageDto) dto.MapToFullDto());
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
