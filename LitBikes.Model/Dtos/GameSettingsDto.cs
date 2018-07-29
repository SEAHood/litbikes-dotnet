using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class GameSettingsDto : IDto
    {
        public int GameTickMs { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new GameSettingsDtoShort
            {
                Gt = GameTickMs
            };
            return shortDto;
        }
    }
}
