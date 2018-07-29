using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Model.Dtos
{
    public static class DebugDtoShortMapper
    {
        public static DebugDtoShort MapToShortDto(this DebugDto dto)
        {
            var shortDto = new DebugDtoShort
            {
                I = dto.Impacts.Select(i => i.MapToShortDto()).ToList()
            };
            return shortDto;
        }

        public static DebugDto MapToFullDto(this DebugDtoShort shortDto)
        {
            var dto = new DebugDto
            {
                Impacts = shortDto.I.Select(i => i.MapToFullDto()).ToList()
            };
            return dto;
        }
    }
}
