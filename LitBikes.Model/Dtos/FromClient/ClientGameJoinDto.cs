using LitBikes.Model.Dtos.FromClient.Short;

namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientGameJoinDto : IDto
    {
        public string Name { get; set; }

        public bool IsValid()
        {
            return Name.Length > 1 || Name.Length <= 15;
        }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ClientGameJoinDtoShort
            {
                N = Name
            };
            return shortDto;
        }
    }
}
