using System;
using LitBikes.Model.Dtos;
using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public interface IClientEventReceiver
    {
    }

    public class ClientEventReceiver : IClientEventReceiver
    {
        public Guid id = Guid.NewGuid();
        public delegate void ClientEventReceiverHandler(object sender, ClientEventReceiverArgs e);
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
            Event?.Invoke(this, new ClientEventReceiverArgs(playerId, ClientEvent.Update));
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
