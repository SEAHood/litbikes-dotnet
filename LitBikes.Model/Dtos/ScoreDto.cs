using System;
namespace LitBikes.Model.Dtos
{
    public class ScoreDto
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
