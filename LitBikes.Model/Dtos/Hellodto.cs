using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class HelloDto : IDto
    {
        public GameSettingsDto GameSettings { get; set; }
        public ServerWorldDto World { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new HelloDtoShort
            {
                Gs = (GameSettingsDtoShort) GameSettings.MapToShortDto(),
                //W = (ServerWorldDtoShort) World.MapToShortDto()
            };
            return shortDto;
        }
    }
}
