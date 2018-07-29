using System;
using System.Collections.Generic;

namespace LitBikes.Model.Dtos
{
    public class ServerWorldDtoShort : IDto
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
    }
}
