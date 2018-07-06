namespace LitBikes.Model.Dtos
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
