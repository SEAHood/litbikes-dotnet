using System.Numerics;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class PowerUpDto : IDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector2 Pos { get; set; }
        public PowerUpType Type { get; set; }
        public bool Collected { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new PowerUpDtoShort
            {
                I = Id,
                N = Name,
                T = Type,
                P = Pos,
                C = Collected
            };
            return shortDto;
        }
    }
}
