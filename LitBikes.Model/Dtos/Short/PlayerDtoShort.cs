using System;

namespace LitBikes.Model.Dtos.Short
{
    public class PlayerDtoShort : IDtoShort
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

        public IDto MapToFullDto()
        {
            var dto = new PlayerDto
            {
                PlayerId = I,
                Name = N,
                Bike = (BikeDto) B.MapToFullDto(),
                Crashed = C,
                CrashedInto = Ci,
                CrashedIntoName = Cin,
                CurrentPowerUp = Cpu,
                Effect = E,
                Spectating = S,
                Score = Sc
            };
            return dto;
        }
    }
}
