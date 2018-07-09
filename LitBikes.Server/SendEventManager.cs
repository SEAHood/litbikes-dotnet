using LitBikes.Events;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public class SendEventManager
    {
        public SendEventManager(IHubContext<SignalHub> hubContext, IServerEventSender eventSender)
        {
            eventSender.Event += async (sender, args) =>
            {
                await SendHub.SendEvent(hubContext.Clients, args);
            };
        }
    }
}
