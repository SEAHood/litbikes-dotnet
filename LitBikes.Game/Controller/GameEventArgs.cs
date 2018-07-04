using System;

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

        public GameEventArgs(GameEvent e)
        {
            Event = e;
        }
    }
}
