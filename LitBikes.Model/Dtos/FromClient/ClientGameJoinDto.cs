namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientGameJoinDto : IDto
    {
        public string Name;

        public bool IsValid()
        {
            return Name.Length > 1 || Name.Length <= 15;
        }
    }
}
