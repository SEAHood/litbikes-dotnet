using System;
using System.Collections.Generic;
using LitBikes.Game.Engine;
using LitBikes.Model;
using LitBikes.Model.Dtos;

namespace LitBikes.Game.Controller
{
    public class GameEventController
    {
        public delegate void GameEventHandler(object sender, GameEventArgs e);
        public event GameEventHandler Event;

        public void PlayerCrashed(Player player)
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.PlayerCrashed, player));
        }

        public void PlayerSpawned()
        {
            Event?.Invoke(this, new GameEventArgs(GameEvent.PlayerSpawned));
        }

        public void ScoreUpdated(List<ScoreDto> scores)
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
