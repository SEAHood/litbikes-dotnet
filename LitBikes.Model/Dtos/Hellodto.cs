namespace LitBikes.Model.Dtos
{
    public class HelloDto : IDto
    {
        public GameSettingsDto GameSettings { get; set; }
        public ServerWorldDto World { get; set; }
    }
}
