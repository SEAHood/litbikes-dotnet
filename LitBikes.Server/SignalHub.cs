using System;
using LitBikes.Game.Controller;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LitBikes.Server
{
    public class SignalHub : Hub
    {
        private readonly ClientEventController _clientEventController;

        public SignalHub(ClientEventController clientEventController)
        {
            _clientEventController = clientEventController;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        #region ClientEvents

        public void Hello()
        {
            _clientEventController.Hello();
        }

        public void RequestJoinGame()
        {
            _clientEventController.RequestJoinGame();
        }

        public void KeepAlive()
        {
            _clientEventController.KeepAlive();
        }

        public void RequestRespawn()
        {
            _clientEventController.RequestRespawn();
        }

        public void Update()
        {
            _clientEventController.Update();
        }

        public void ChatMessage()
        {
            _clientEventController.ChatMessage();
        }

        public void UsePowerup()
        {
            _clientEventController.UsePowerup();
        }

        #endregion



        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
