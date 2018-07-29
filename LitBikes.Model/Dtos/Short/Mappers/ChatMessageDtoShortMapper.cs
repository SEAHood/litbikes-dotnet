using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public static class ChatMessageDtoShortMapper
    {
        public static ChatMessageDtoShort MapToShortDto(this ChatMessageDto dto)
        {
            var shortDto = new ChatMessageDtoShort
            {
                S = dto.Source,
                Ism = dto.IsSystemMessage,
                M = dto.Message,
                Sc = dto.SourceColour,
                T = dto.Timestamp
            };
            return shortDto;
        }

        public static ChatMessageDto MapToFullDto(this ChatMessageDtoShort shortDto)
        {
            var dto = new ChatMessageDto
            {
                Source = shortDto.S,
                IsSystemMessage = shortDto.Ism,
                Message = shortDto.M,
                SourceColour = shortDto.Sc,
                Timestamp = shortDto.T
            };
            return dto;
        }
    }
}
