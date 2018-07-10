using System;
namespace LitBikes.Model.Dtos
{
    public class ScoreDto
    {
        public Guid PlayerId;
        public string Name;
        public int Score;

        public ScoreDto(Guid playerId, string name, int score)
        {
            PlayerId = playerId;
            Name = name;
            Score = score;
        }
    }
}
