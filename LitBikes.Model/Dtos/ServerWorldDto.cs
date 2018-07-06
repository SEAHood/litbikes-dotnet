using System;
using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class ServerWorldDto : IDto
    {
        public long timestamp;
        public long gameTick;
        public bool roundInProgress;
        public int timeUntilNextRound;
        public Guid currentWinner;
        public int roundTimeLeft;
        public List<PlayerDto> players;
        public List<PowerUpDto> powerUps;
        public ArenaDto arena;
        public DebugDto debug;
    }
}
