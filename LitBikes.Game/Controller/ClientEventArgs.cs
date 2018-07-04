using System;

namespace LitBikes.Game.Controller
{
    public enum ClientEvent
    {
        Hello,
        RequestJoinGame,
        KeepAlive,
        RequestRespawn,
        Update,
        ChatMessage,
        UsePowerup
    }

    public class ClientEventArgs : EventArgs
    {
        public ClientEvent Event { get; }

        public ClientEventArgs(ClientEvent e)
        {
            Event = e;
        }
    }
}
