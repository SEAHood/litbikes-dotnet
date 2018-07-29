using System.Numerics;

namespace LitBikes.Model.Dtos.Short
{
    public class PowerUpDtoShort : IDtoShort
    {
        public string I { get; set; }
        public string N { get; set; }
        public Vector2 P { get; set; }
        public PowerUpType T { get; set; }
        public bool C { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new PowerUpDto
            {
                Id = I,
                Name = N,
                Type = T,
                Pos = P,
                Collected = C
            };
            return dto;
        }
    }
}
