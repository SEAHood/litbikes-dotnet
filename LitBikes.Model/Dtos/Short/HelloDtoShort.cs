namespace LitBikes.Model.Dtos.Short
{
    public class HelloDtoShort : IDtoShort
    {
        public GameSettingsDtoShort Gs { get; set; }
        public ServerWorldDtoShort W { get; set; }
        
        public IDto  MapToFullDto()
        {
            var dto = new HelloDto
            {
                GameSettings = (GameSettingsDto) Gs.MapToFullDto(),
                World = (ServerWorldDto) W.MapToFullDto()
            };
            return dto;
        }
    }
}
