using System;
using LitBikes.Model.Dtos.FromClient;
using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public delegate void ClientEventReceiverHandler(object sender, ClientEventReceiverArgs e);

    public interface IClientEventReceiver
    {
        event ClientEventReceiverHandler Event;
        void Hello(Guid playerId);
        void RequestJoinGame(Guid playerId, ClientGameJoinDto dto);
        void KeepAlive(Guid playerId);
        void RequestRespawn(Guid playerId);
        void Update(Guid playerId, ClientUpdateDto dto);
        void ChatMessage(Guid playerId, ClientChatMessageDto dto);
        void UsePowerup(Guid playerId);
    }

    public class ClientEventReceiver : IClientEventReceiver
    {
        public event ClientEventReceiverHandler Event;

        public void Hello(Guid playerId)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.Hello));
        }

        public void RequestJoinGame(Guid playerId, ClientGameJoinDto dto)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.RequestJoinGame, dto));
        }

        public void KeepAlive(Guid playerId)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.KeepAlive));
        }

        public void RequestRespawn(Guid playerId)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.RequestRespawn));
        }

        public void Update(Guid playerId, ClientUpdateDto dto)
        {
            dto.PlayerId = playerId; // TODO Refactor this out, just give engine the ID
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.Update, dto));
        }

        public void ChatMessage(Guid playerId, ClientChatMessageDto dto)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.ChatMessage, dto));
        }

        public void UsePowerup(Guid playerId)
        {
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.UsePowerup));
        }
    }
}
