using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Model.Dtos.Short
{
    public class DebugDtoShort : IDtoShort
    {
        public List<ImpactDtoShort> I;

        public IDto MapToFullDto()
        {
            var dto = new DebugDto
            {
                Impacts = I.Select(i => (ImpactDto) i.MapToFullDto()).ToList()
            };
            return dto;
        }
    }
}
