using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class ServerWorldDto
    {
        public long timestamp;
        public long gameTick;
        public bool roundInProgress;
        public int timeUntilNextRound;
        public int currentWinner;
        public int roundTimeLeft;
        public List<PlayerDto> players;
        public List<PowerUpDto> powerUps;
        public ArenaDto arena;
        public DebugDto debug;
    }
}
