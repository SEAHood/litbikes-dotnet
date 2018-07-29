using System;
using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Model.Dtos.Short
{
    public class ServerWorldDtoShort : IDtoShort
    {
        public long T { get; set; }
        public long Gt { get; set; }
        public bool Rip { get; set; }
        public int Tunr { get; set; }
        public Guid Cw { get; set; }
        public int Rtl { get; set; }
        public List<PlayerDtoShort> P { get; set; }
        public List<PowerUpDtoShort> Pu { get; set; }
        public ArenaDtoShort A { get; set; }
        public DebugDtoShort D { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new ServerWorldDto
            {
                Arena = (ArenaDto) A.MapToFullDto(),
                CurrentWinner = Cw,
                Debug = (DebugDto) D.MapToFullDto(),
                GameTick = Gt,
                Players = P.Select(p => (PlayerDto) p.MapToFullDto()).ToList(),
                PowerUps = Pu.Select(pu => (PowerUpDto) pu.MapToFullDto()).ToList(),
                RoundInProgress = Rip,
                RoundTimeLeft = Rtl,
                Timestamp = T,
                TimeUntilNextRound = Tunr
            };
            return dto;
        }
    }
}
