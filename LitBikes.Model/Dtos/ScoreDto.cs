using System;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class ScoreDto : IDto
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ScoreDtoShort
            {
                I = PlayerId,
                N = Name,
                S = Score
            };
            return shortDto;
        }
    }
}
