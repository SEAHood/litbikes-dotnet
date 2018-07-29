using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public static class GameSettingsDtoShortMapper
    {
        public static GameSettingsDtoShort MapToShortDto(this GameSettingsDto dto)
        {
            var shortDto = new GameSettingsDtoShort
            {
                Gt = dto.GameTickMs
            };
            return shortDto;
        }

        public static GameSettingsDto MapToFullDto(this GameSettingsDtoShort shortDto)
        {
            var dto = new GameSettingsDto
            {
                GameTickMs = shortDto.Gt
            };
            return dto;
        }
    }
}
