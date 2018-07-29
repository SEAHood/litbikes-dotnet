namespace LitBikes.Model.Dtos.Short
{
    public class GameSettingsDtoShort : IDtoShort
    {
        public int Gt { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new GameSettingsDto
            {
                GameTickMs = Gt
            };
            return dto;
        }
    }
}
