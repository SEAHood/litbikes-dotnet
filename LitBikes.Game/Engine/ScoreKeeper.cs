using System;
using System.Collections.Generic;
using System.Linq;
using LitBikes.Model.Dtos;

namespace LitBikes.Game.Engine
{
    public class ScoreKeeper
    {
        private readonly List<ScoreDto> _scores;

        public ScoreKeeper()
        {
            _scores = new List<ScoreDto>();
        }

        public void GrantScore(Guid pid, string name, int score)
        {
            var currentScore = _scores.FirstOrDefault(s => s.PlayerId == pid);
            if (currentScore == null)
            {
                currentScore = new ScoreDto
                {
                    Name = name,
                    PlayerId = pid,
                    Score = 0
                };
                _scores.Add(currentScore);
            }
            currentScore.Score = currentScore.Score + score;
        }

        public int GetScore(Guid playerId)
        {
            return _scores.FirstOrDefault(s => s.PlayerId == playerId)?.Score ?? 0;
        }

        public void RemoveScore(Guid playerId)
        {
            var currentScore = _scores.FirstOrDefault(s => s.PlayerId == playerId);
            _scores.Remove(currentScore);
        }

        public List<ScoreDto> GetScores()
        {
            return _scores;
        }

        public void Reset()
        {
            _scores.Clear();
        }

        public Guid GetCurrentWinner()
        {
            return !_scores.Any() ? Guid.Empty : _scores.ToList().OrderBy(s => s.Score).First().PlayerId;
        }

        public string GetCurrentWinnerName()
        {
            return !_scores.Any() ? "Unknown" : _scores.ToList().OrderBy(s => s.Score).First().Name;
        }

    }
}
