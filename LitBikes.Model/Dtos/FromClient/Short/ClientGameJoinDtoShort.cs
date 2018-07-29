namespace LitBikes.Model.Dtos.FromClient.Short
{
    public class ClientGameJoinDtoShort : IDtoShort
    {
        public string N { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new ClientGameJoinDto
            {
                Name = N
            };
            return dto;
        }
    }
}
