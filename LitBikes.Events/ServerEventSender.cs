using System;
using System.Collections.Generic;
using System.Linq;
using LitBikes.Model;
using LitBikes.Model.Dtos;
using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public delegate void ServerSendEventHandler(object sender, ServerEventSenderArgs e);

    public interface IServerEventSender
    {
        event ServerSendEventHandler Event;
        void SendEvent(ServerEvent e, Guid? playerId);
        void SendEvent(ServerEvent e, IDto payload, Guid? playerId);
        void SendListEvent(ServerEvent e, List<IDto> payload, Guid? playerId);
    }

    public class ServerEventSender : IServerEventSender
    {
        public event ServerSendEventHandler Event;

        public void SendEvent(ServerEvent e, Guid? playerId)
        {
            Event?.Invoke(this, new ServerEventSenderArgs(e, null, playerId));
        }

        public void SendEvent(ServerEvent e, IDto payload, Guid? playerId)
        {
            var shortenedPayload = payload.MapToShortDto();
            Event?.Invoke(this, new ServerEventSenderArgs(e, shortenedPayload, playerId));
        }

        public void SendListEvent(ServerEvent e, List<IDto> payload, Guid? playerId)
        {
            var shortenedPayload = payload.Select(p => p.MapToShortDto()).ToList();
            Event?.Invoke(this, new ServerEventSenderArgs(e, shortenedPayload, playerId));
        }
    }
}
