using System;
using LitBikes.Model;

namespace LitBikes.Game.Controller
{
    public enum GameEvent
    {
        PlayerCrashed,
        PlayerSpawned,
        ScoreUpdated,
        GameStarted,
        RoundStarted,
        RoundEnded
        /*void playerCrashed(Player player);
        void playerSpawned(int pid);
        void scoreUpdated(List<ScoreDto> scores);
        void gameStarted();
        void roundStarted();
        void roundEnded();*/
    }

    public class GameEventArgs : EventArgs
    {
        public GameEvent Event { get; }
        public Player Player;

        public GameEventArgs(GameEvent e)
        {
            Event = e;
        }

        public GameEventArgs(GameEvent e, Player player)
        {
            Event = e;
            Player = player;
        }
    }
}
