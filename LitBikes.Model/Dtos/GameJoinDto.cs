using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class GameJoinDto : IDto
    {
        public List<ScoreDto> Scores { get; set; }
        public PlayerDto Player { get; set; }
    }
}