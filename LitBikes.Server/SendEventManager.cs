using System;
using System.Threading.Tasks;
using LitBikes.Events;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public class SendEventManager
    {
        public SendEventManager(IHubContext<SignalHub> hubContext, IServerEventSender eventSender)
        {
            eventSender.Event += (sender, args) =>
            {
                SendHub.SendEvent(hubContext.Clients, args);
            };
        }
    }
}
