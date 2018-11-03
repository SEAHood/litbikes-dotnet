using System;

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
    }
}
