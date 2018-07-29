using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class GameJoinDtoShort : IDto
    {
        public List<ScoreDtoShort> S { get; set; }
        public PlayerDtoShort P { get; set; }
    }
}
