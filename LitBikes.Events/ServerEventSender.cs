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
        void SendEvent(ServerEvent e);
        void SendEvent(ServerEvent e, IDto payload);
        void SendListEvent(ServerEvent e, List<IDto> payload);
    }

    public class ServerEventSender : IServerEventSender
    {
        public event ServerSendEventHandler Event;

        public void SendEvent(ServerEvent e)
        {
            Event?.Invoke(this, new ServerEventSenderArgs(e, null));
        }

        public void SendEvent(ServerEvent e, IDto payload)
        {
            var shortenedPayload = payload.MapToShortDto();
            Event?.Invoke(this, new ServerEventSenderArgs(e, shortenedPayload));
        }

        public void SendListEvent(ServerEvent e, List<IDto> payload)
        {
            var shortenedPayload = payload.Select(p => p.MapToShortDto()).ToList();
            Event?.Invoke(this, new ServerEventSenderArgs(e, shortenedPayload));
        }
    }
}
