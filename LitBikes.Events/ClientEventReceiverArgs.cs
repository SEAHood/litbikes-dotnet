using LitBikes.Model.Enums;
using System;
using LitBikes.Model;

namespace LitBikes.Events
{
    public class ClientEventReceiverArgs : EventArgs
    {
        public ClientEvent Event { get; }
        public Guid PlayerId { get; }
        public IDto Dto { get; }

        public ClientEventReceiverArgs(Guid playerId, ClientEvent clientEvent)
            : this(playerId, clientEvent, null) { }

        public ClientEventReceiverArgs(Guid playerId, ClientEvent clientEvent, IDto dto)
        {
            PlayerId = playerId;
            Event = clientEvent;
            Dto = dto;
        }
    }
}
