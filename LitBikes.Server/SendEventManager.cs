using LitBikes.Events;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public class SendEventManager
    {
        private IServerEventSender _eventSender;

        public SendEventManager(IHubContext<SignalHub> hubContext, IServerEventSender eventSender)
        {
            _eventSender = eventSender;
            _eventSender.Event += async (sender, args) =>
            {
                await SendHub.SendEvent(hubContext.Clients, args);
            };
        }
    }
}
