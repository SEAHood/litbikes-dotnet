namespace LitBikes.Model.Dtos
{
    public class HelloDtoShort : IDto
    {
        public GameSettingsDtoShort Gs { get; set; }
        public ServerWorldDtoShort W { get; set; }
    }
}
