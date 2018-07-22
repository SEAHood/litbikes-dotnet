using System;

namespace LitBikes.Model.Dtos
{
    public class PlayerDto
    {
        public Guid PlayerId;
        public string Name;
        public BikeDto Bike;
        public bool Spectating;
        public bool Crashed;
        public Guid? CrashedInto;
        public string CrashedIntoName;
        public int Score;
        public PowerUpType CurrentPowerUp;
        public PlayerEffect Effect;
    }
}
