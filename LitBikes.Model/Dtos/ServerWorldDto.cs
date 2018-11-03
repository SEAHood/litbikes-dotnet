using System;
using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class ServerWorldDto : IDto
    {
        public long Timestamp { get; set; }
        public long GameTick { get; set; }
        public bool RoundInProgress { get; set; }
        public int TimeUntilNextRound { get; set; }
        public Guid CurrentWinner { get; set; }
        public int RoundTimeLeft { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<PowerUpDto> PowerUps { get; set; }
        public ArenaDto Arena { get; set; }
        public DebugDto Debug { get; set; }
    }
}
