using System;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class PlayerDto : IDto
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public BikeDto Bike { get; set; }
        public bool Spectating { get; set; }
        public bool Crashed { get; set; }
        public Guid? CrashedInto { get; set; }
        public string CrashedIntoName { get; set; }
        public int Score { get; set; }
        public PowerUpType CurrentPowerUp { get; set; }
        public PlayerEffect Effect { get; set; }
        
        public IDtoShort MapToShortDto()
        {
            var shortDto = new PlayerDtoShort
            {
                I = PlayerId,
                N = Name,
                B = (BikeDtoShort) Bike.MapToShortDto(),
                C = Crashed,
                Ci = CrashedInto,
                Cin = CrashedIntoName,
                Cpu = CurrentPowerUp,
                E = Effect,
                S = Spectating,
                Sc = Score
            };
            return shortDto;
        }
    }
}
