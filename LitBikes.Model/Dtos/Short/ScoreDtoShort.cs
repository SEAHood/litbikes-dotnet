using System;

namespace LitBikes.Model.Dtos.Short
{
    public class ScoreDtoShort : IDtoShort
    {
        public Guid I { get; set; }
        public string N { get; set; }
        public int S { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new ScoreDto
            {
                PlayerId = I,
                Name = N,
                Score = S
            };
            return dto;
        }
    }
}
