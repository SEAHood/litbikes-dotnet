namespace LitBikes.Model.Dtos.FromClient.Short
{
    public class ClientChatMessageDtoShort : IDtoShort
    {
        public string M { get; set; }
        
        public IDto MapToFullDto()
        {
            var dto = new ClientChatMessageDto
            {
                Message = M
            };
            return dto;
        }
    }
}
