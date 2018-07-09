namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientGameJoinDto : IDto
    {
        public string name;

        public bool IsValid()
        {
            return name.Length > 1 || name.Length <= 15;
        }
    }
}
