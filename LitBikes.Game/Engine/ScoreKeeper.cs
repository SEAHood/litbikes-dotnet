using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LitBikes.Model.Dtos;

namespace LitBikes.Game.Engine
{
    public class ScoreKeeper
    {
        private readonly ConcurrentDictionary<Guid, ScoreDto> _scores;
        //private static readonly Object _scoreLock = new object();

        public ScoreKeeper()
        {
            _scores = new ConcurrentDictionary<Guid, ScoreDto>();
        }

        public void GrantScore(Guid pid, string name, int score)
        {
            //var currentScore = _scores.FirstOrDefault(s => s.PlayerId == pid);
            _scores.TryGetValue(pid, out var currentScore);
            if (currentScore == null)
            {
                currentScore = new ScoreDto
                {
                    Name = name,
                    PlayerId = pid,
                    Score = 0
                };
                //_scores.Add(currentScore);
                _scores.TryAdd(pid, currentScore);
            }
            currentScore.Score = currentScore.Score + score;
        }

        public int GetScore(Guid playerId)
        {
            _scores.TryGetValue(playerId, out var scoreObj);
            return scoreObj?.Score ?? 0;
        }

        public void RemoveScore(Guid playerId)
        {
            _scores.TryRemove(playerId, out _);
        }

        public List<ScoreDto> GetScores()
        {
            return _scores.Values.ToList();
        }

        public void Reset()
        {
            _scores.Clear();
        }

        public Guid GetCurrentWinner()
        {
            var scores = _scores.Values.ToList();
            return !scores.Any() ? Guid.Empty : scores.OrderByDescending(s => s.Score).First().PlayerId;
        }

        public string GetCurrentWinnerName()
        {
            var scores = _scores.Values.ToList();
            return !scores.Any() ? "Unknown" : scores.OrderByDescending(s => s.Score).First().Name;
        }

    }
}
