using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public static class HelloDtoShortMapper
    {
        public static HelloDtoShort MapFromHelloDto(this HelloDto dto)
        {
            var shortDto = new HelloDtoShort
            {
                Gs = dto.GameSettings.MapToShortDto(),
                W = dto.World.MapToShortDto()
            };
            return shortDto;
        }

        public static HelloDto MapToHelloDto(this HelloDtoShort shortDto)
        {
            var dto = new HelloDto
            {
                GameSettings = shortDto.Gs.MapToFullDto(),
                World = shortDto.W.MapToFullDto()
            };
            return dto;
        }
    }
}
