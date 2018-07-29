using System;
using System.Collections.Generic;
using System.Linq;
using LitBikes.Model.Dtos.Short;

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

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ServerWorldDtoShort
            {
                A = (ArenaDtoShort) Arena.MapToShortDto(),
                Cw = CurrentWinner,
                D = (DebugDtoShort) Debug.MapToShortDto(),
                Gt = GameTick,
                P = Players.Select(p => (PlayerDtoShort) p.MapToShortDto()).ToList(),
                Pu = PowerUps.Select(pu => (PowerUpDtoShort) pu.MapToShortDto()).ToList(),
                Rip = RoundInProgress,
                Rtl = RoundTimeLeft,
                T = Timestamp,
                Tunr = TimeUntilNextRound
            };
            return shortDto;
        }
    }
}
