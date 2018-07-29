using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public static class ImpactDtoShortMapper
    {
        public static ImpactDtoShort MapToShortDto(this ImpactDto dto)
        {
            var shortDto = new ImpactDtoShort
            {
                P = dto.Pos
            };
            return shortDto;
        }

        public static ImpactDto MapToFullDto(this ImpactDtoShort shortDto)
        {
            var dto = new ImpactDto
            {
                Pos = shortDto.P
            };
            return dto;
        }
    }
}
