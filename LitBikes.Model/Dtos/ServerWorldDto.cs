using System;
using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class ServerWorldDto : IDto
    {
        public long Timestamp;
        public long GameTick;
        public bool RoundInProgress;
        public int TimeUntilNextRound;
        public Guid CurrentWinner;
        public int RoundTimeLeft;
        public List<PlayerDto> Players;
        public List<PowerUpDto> PowerUps;
        public ArenaDto Arena;
        public DebugDto Debug;
    }
}
