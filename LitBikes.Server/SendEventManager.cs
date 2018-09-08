using System;
using System.Threading.Tasks;
using LitBikes.Events;
using Microsoft.AspNetCore.SignalR;

namespace LitBikes.Server
{
    public class SendEventManager
    {
        //private static DateTime lastEvent;
        public SendEventManager(IHubContext<SignalHub> hubContext, IServerEventSender eventSender)
        {
            eventSender.Event += (sender, args) =>
            {
                //var date = DateTime.UtcNow;
                //var dur = date - lastEvent;
                //Console.WriteLine($"Time since last event sender event: {dur.TotalMilliseconds}ms");
                //lastEvent = date;
                SendHub.SendEvent(hubContext.Clients, args);


                //new Task(async () => await SendHub.SendEvent(hubContext.Clients, args)).Start();
            };
        }
    }
}
