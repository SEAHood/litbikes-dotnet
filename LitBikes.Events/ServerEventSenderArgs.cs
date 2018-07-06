using System;
using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public class ServerEventSenderArgs : EventArgs
    {
        public ServerEvent Event { get; }
        public object Payload { get; }

        public ServerEventSenderArgs(ServerEvent serverEvent, object payload)
        {
            Event = serverEvent;
            Payload = payload;
        }
    }
}
