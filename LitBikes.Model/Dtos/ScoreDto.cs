using System;

namespace LitBikes.Model.Dtos
{
    public class ScoreDto : IDto
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
