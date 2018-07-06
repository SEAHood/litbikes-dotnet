using LitBikes.Model.Enums;

namespace LitBikes.Events
{
    public class ServerEventSender
    {
        public delegate void ServerSendEventHandler(object sender, ServerEventSenderArgs e);
        public event ServerSendEventHandler Event;
        
        public void SendEvent(ServerEvent serverEvent, object payload)
        {
            Event?.Invoke(this, new ServerEventSenderArgs(serverEvent, payload));
        }
    }
}
