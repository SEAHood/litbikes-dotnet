namespace LitBikes.Model.Dtos.Short
{
    public class ChatMessageDtoShort : IDtoShort
    {
        public long T { get; set; }
        public string S { get; set; }
        public string Sc { get; set; }
        public string M { get; set; }
        public bool Ism { get; set; }

        public IDto MapToFullDto()
        {
            var dto = new ChatMessageDto
            {
                Source = S,
                IsSystemMessage = Ism,
                Message = M,
                SourceColour = Sc,
                Timestamp = T
            };
            return dto;
        }
    }
}
