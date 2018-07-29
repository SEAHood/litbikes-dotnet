using System.Numerics;

namespace LitBikes.Model.Dtos.Short
{
    public class ImpactDtoShort : IDtoShort
    {
        public Vector2 P;

        public IDto MapToFullDto()
        {
            var dto = new ImpactDto
            {
                Pos = P
            };
            return dto;
        }
    }
}
