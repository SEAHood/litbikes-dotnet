using System;

namespace LitBikes.Model.Dtos
{
    public class PlayerDtoShort : IDto
    {
        public Guid I { get; set; }
        public string N { get; set; }
        public BikeDtoShort B { get; set; }
        public bool S { get; set; }
        public bool C { get; set; }
        public Guid? Ci { get; set; }
        public string Cin { get; set; }
        public int Sc { get; set; }
        public PowerUpType Cpu { get; set; }
        public PlayerEffect E { get; set; }
    }
}
