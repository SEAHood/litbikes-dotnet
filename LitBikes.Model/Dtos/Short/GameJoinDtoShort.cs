using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Model.Dtos.Short
{
    public class GameJoinDtoShort : IDtoShort
    {
        public List<ScoreDtoShort> S { get; set; }
        public PlayerDtoShort P { get; set; }


        public IDto MapToFullDto()
        {
            var shortDto = new GameJoinDto
            {
                Player = (PlayerDto) P.MapToFullDto(),
                Scores = S.Select(s => (ScoreDto) s.MapToFullDto()).ToList()
            };
            return shortDto;
        }
    }
}
