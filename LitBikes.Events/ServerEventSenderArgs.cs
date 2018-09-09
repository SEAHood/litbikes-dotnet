using System;
using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public class ServerEventSenderArgs : EventArgs
    {
        public Guid? PlayerId { get; set; }
        public ServerEvent Event { get; }
        public object Payload { get; }

        public ServerEventSenderArgs(ServerEvent serverEvent, object payload, Guid? playerId)
        {
            Event = serverEvent;
            Payload = payload;
            PlayerId = playerId;
        }
    }
}
