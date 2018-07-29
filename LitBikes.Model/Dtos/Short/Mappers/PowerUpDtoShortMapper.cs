using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public static class PowerUpDtoShortMapper
    {
        public static PowerUpDtoShort MapToShortDto(this PowerUpDto dto)
        {
            var shortDto = new PowerUpDtoShort
            {
                I = dto.Id,
                N = dto.Name,
                T = dto.Type,
                P = dto.Pos,
                C = dto.Collected
            };
            return shortDto;
        }

        public static PowerUpDto MapToFullDto(this PowerUpDtoShort shortDto)
        {
            var dto = new PowerUpDto
            {
                Id = shortDto.I,
                Name = shortDto.N,
                Type = shortDto.T,
                Pos = shortDto.P,
                Collected = shortDto.C
            };
            return dto;
        }
    }
}

