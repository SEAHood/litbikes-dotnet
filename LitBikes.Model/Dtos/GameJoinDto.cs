using System.Collections.Generic;
using System.Linq;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class GameJoinDto : IDto
    {
        public List<ScoreDto> Scores { get; set; }
        public PlayerDto Player { get; set; }
        
        public IDtoShort MapToShortDto()
        {
            var shortDto = new GameJoinDtoShort
            {
                P = (PlayerDtoShort) Player.MapToShortDto(),
                S = Scores.Select(s => (ScoreDtoShort) s.MapToShortDto()).ToList()
            };
            return shortDto;
        }
    }
}