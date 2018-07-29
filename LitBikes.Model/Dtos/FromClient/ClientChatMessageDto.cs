using LitBikes.Model.Dtos.FromClient.Short;

namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientChatMessageDto : IDto
    {
        public string Message { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ClientChatMessageDtoShort
            {
                M = Message
            };
            return shortDto;
        }
    }
}