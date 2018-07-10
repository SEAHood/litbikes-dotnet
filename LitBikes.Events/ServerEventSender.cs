using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public delegate void ServerSendEventHandler(object sender, ServerEventSenderArgs e);

    public interface IServerEventSender
    {
        event ServerSendEventHandler Event;
        void SendEvent(ServerEvent e);
        void SendEvent(ServerEvent e, object payload);
    }

    public class ServerEventSender : IServerEventSender
    {
        public event ServerSendEventHandler Event;

        public void SendEvent(ServerEvent e) {
            SendEvent(e, null);
        }

        public void SendEvent(ServerEvent e, object payload)
        {
            Event?.Invoke(this, new ServerEventSenderArgs(e, payload));
        }
    }
}
