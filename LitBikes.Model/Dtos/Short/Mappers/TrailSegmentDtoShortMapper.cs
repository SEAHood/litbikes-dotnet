using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public static class TrailSegmentDtoShortMapper
    {
        public static TrailSegmentDtoShort MapToShortDto(this TrailSegmentDto dto)
        {
            var shortDto = new TrailSegmentDtoShort
            {
                S = dto.Start,
                E = dto.End,
                Ih = dto.IsHead
            };
            return shortDto;
        }

        public static TrailSegmentDto MapToFullDto(this TrailSegmentDtoShort shortDto)
        {
            var dto = new TrailSegmentDto
            {
                Start = shortDto.S,
                End = shortDto.E,
                IsHead = shortDto.Ih
            };
            return dto;
        }
    }
}
