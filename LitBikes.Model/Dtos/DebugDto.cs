using System.Collections.Generic;
using System.Linq;
using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class DebugDto : IDto
    {
        public List<ImpactDto> Impacts;

        public IDtoShort MapToShortDto()
        {
            var shortDto = new DebugDtoShort
            {
                I = Impacts.Select(i => (ImpactDtoShort) i.MapToShortDto()).ToList()
            };
            return shortDto;
        }
    }
}
