namespace LitBikes.Game.Controller
{
    public class ClientEventController
    {
        public delegate void ClientEventHandler(object sender, ClientEventArgs e);
        public event ClientEventHandler Event;

        public void Hello()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.Hello));
        }

        public void RequestJoinGame()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.RequestJoinGame));
        }

        public void KeepAlive()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.KeepAlive));
        }

        public void RequestRespawn()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.RequestRespawn));
        }

        public void Update()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.Update));
        }

        public void ChatMessage()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.ChatMessage));
        }

        public void UsePowerup()
        {
            Event?.Invoke(this, new ClientEventArgs(ClientEvent.UsePowerup));
        }
    }
}
