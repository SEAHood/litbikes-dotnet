
namespace LitBikes.Model.Dtos
{
    public static class ArenaDtoShortMapper
    {
        public static ArenaDtoShort MapToShortDto(this ArenaDto dto)
        {
            var shortDto = new ArenaDtoShort
            {
                S = dto.Size
            };
            return shortDto;
        }

        public static ArenaDto MapToFullDto(this ArenaDtoShort shortDto)
        {
            var dto = new ArenaDto
            {
                Size = shortDto.S
            };
            return dto;
        }
    }
}
