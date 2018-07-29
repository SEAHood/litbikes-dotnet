using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace LitBikes.Model.Dtos
{
    public static class BikeDtoShortMapper
    {
        public static BikeDtoShort MapToShortDto(this BikeDto dto)
        {
            var shortDto = new BikeDtoShort
            {
                D = dto.Dir,
                C = dto.Colour,
                P = dto.Pos,
                S = dto.Spd,
                T = dto.Trail.Select(t => t.MapToShortDto()).ToList()
            };
            return shortDto;
        }

        public static BikeDto MapToFullDto(this BikeDtoShort shortDto)
        {
            var dto = new BikeDto
            {
                Dir = shortDto.D,
                Colour = shortDto.C,
                Pos = shortDto.P,
                Spd = shortDto.S,
                Trail = shortDto.T.Select(t => t.MapToFullDto()).ToList()
            };
            return dto;
        }
    }
}
