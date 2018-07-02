using System;
using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Game
{
    public class ScoreDto
    {
        public int pid;
        public String name;
        public int score;

        public ScoreDto(int _pid, String _name, int _score)
        {
            pid = _pid;
            name = _name;
            score = _score;
        }
    }

    public class ScoreKeeper
    {
        private readonly List<ScoreDto> scores;

        public ScoreKeeper()
        {
            scores = new List<ScoreDto>();
        }

        public void GrantScore(int pid, String name, int score)
        {
            var currentScore = scores.FirstOrDefault(s => s.pid == pid);
            if (currentScore == null)
            {
                currentScore = new ScoreDto(pid, name, 0);
                scores.Add(currentScore);
            }
            currentScore.score = currentScore.score + score;
        }

        public int GetScore(int pid)
        {
            return scores.FirstOrDefault(s => s.pid == pid)?.score ?? 0;
        }

        public void RemoveScore(int pid)
        {
            var currentScore = scores.FirstOrDefault(s => s.pid == pid);
            scores.Remove(currentScore);
        }

        public List<ScoreDto> GetScores()
        {
            return scores;
        }

        public void Reset()
        {
            scores.Clear();
        }

        public int GetCurrentWinner()
        {
            if (!scores.Any()) return -1;
            return scores.OrderBy(s => s.score).First().pid;
        }

        public String GetCurrentWinnerName()
        {
            if (!scores.Any()) return "Unknown";
            return scores.OrderBy(s => s.score).First().name;
        }

    }
}
