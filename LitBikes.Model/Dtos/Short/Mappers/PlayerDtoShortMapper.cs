using System;

namespace LitBikes.Model.Dtos
{
    public static class PlayerDtoShortMapper
    {
        public static PlayerDtoShort MapToShortDto(this PlayerDto dto)
        {
            var shortDto = new PlayerDtoShort
            {
                I = dto.PlayerId,
                N = dto.Name,
                B = dto.Bike.MapToShortDto(),
                C = dto.Crashed,
                Ci = dto.CrashedInto,
                Cin = dto.CrashedIntoName,
                Cpu = dto.CurrentPowerUp,
                E = dto.Effect,
                S = dto.Spectating,
                Sc = dto.Score
            };
            return shortDto;
        }

        public static PlayerDto MapToFullDto(this PlayerDtoShort shortDto)
        {
            var dto = new PlayerDto
            {
                PlayerId = shortDto.I,
                Name = shortDto.N,
                Bike = shortDto.B.MapToFullDto(),
                Crashed = shortDto.C,
                CrashedInto = shortDto.Ci,
                CrashedIntoName = shortDto.Cin,
                CurrentPowerUp = shortDto.Cpu,
                Effect = shortDto.E,
                Spectating = shortDto.S,
                Score = shortDto.Sc
            };
            return dto;
        }
    }
}
