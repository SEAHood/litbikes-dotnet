namespace LitBikes.Model.Dtos.FromClient
{
    public class ClientChatMessageDto : IDto
    {
        public string Message;

        public ClientChatMessageDto(string message)
        {
            Message = message;
        }
    }
}
