using System;
using System.Collections.Generic;
using System.Linq;

namespace LitBikes.Model.Dtos
{
    public static class ServerWorldDtoShortMapper
    {
        public static ServerWorldDtoShort MapToShortDto(this ServerWorldDto dto)
        {
            var shortDto = new ServerWorldDtoShort
            {
                A = dto.Arena.MapToShortDto(),
                Cw = dto.CurrentWinner,
                D = dto.Debug.MapToShortDto(),
                Gt = dto.GameTick,
                P = dto.Players.Select(p => p.MapToShortDto()).ToList(),
                Pu = dto.PowerUps.Select(pu => pu.MapToShortDto()).ToList(),
                Rip = dto.RoundInProgress,
                Rtl = dto.RoundTimeLeft,
                T = dto.Timestamp,
                Tunr = dto.TimeUntilNextRound
            };
            return shortDto;
        }

        public static ServerWorldDto MapToFullDto(this ServerWorldDtoShort shortDto)
        {
            var dto = new ServerWorldDto
            {
                Arena = shortDto.A.MapToFullDto(),
                CurrentWinner = shortDto.Cw,
                Debug = shortDto.D.MapToFullDto(),
                GameTick = shortDto.Gt,
                Players = shortDto.P.Select(p => p.MapToFullDto()).ToList(),
                PowerUps = shortDto.Pu.Select(pu => pu.MapToFullDto()).ToList(),
                RoundInProgress = shortDto.Rip,
                RoundTimeLeft = shortDto.Rtl,
                Timestamp = shortDto.T,
                TimeUntilNextRound = shortDto.Tunr
            };
            return dto;
        }
    }
}
