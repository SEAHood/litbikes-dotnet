namespace LitBikes.Model.Dtos
{
    public static class ScoreDtoShortMapper
    {
        public static ScoreDtoShort MapToShortDto(this ScoreDto dto)
        {
            var shortDto = new ScoreDtoShort
            {
                I = dto.PlayerId,
                N = dto.Name,
                S = dto.Score
            };
            return shortDto;
        }

        public static ScoreDto MapToFullDto(this ScoreDtoShort shortDto)
        {
            var dto = new ScoreDto
            {
                PlayerId = shortDto.I,
                Name = shortDto.N,
                Score = shortDto.S
            };
            return dto;
        }
    }
}
