namespace LitBikes.Game.Controller
{
    public class GameEventController
    {
        public delegate void GameEventHandler(object sender, GameEventArgs e);
        public event GameEventHandler Event;

        public void PlayerCrashed()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.PlayerCrashed));
        }

        public void PlayerSpawned()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.PlayerSpawned));
        }

        public void ScoreUpdated()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.ScoreUpdated));
        }

        public void GameStarted()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.GameStarted));
        }

        public void RoundStarted()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.RoundStarted));
        }

        public void RoundEnded()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.RoundEnded));
        }
    }
}
