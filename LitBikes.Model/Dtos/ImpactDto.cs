using System.Numerics;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class ImpactDto : IDto
    {
        public Vector2 Pos;

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ImpactDtoShort
            {
                P = Pos
            };
            return shortDto;
        }
    }
}
