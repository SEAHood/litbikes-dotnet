using LitBikes.Model.Dtos.Short;

namespace LitBikes.Model.Dtos
{
    public class ChatMessageDto : IDto
    {
        public long Timestamp { get; set; }
        public string Source { get; set; }
        public string SourceColour { get; set; } // in rgba(0,0,0,0) format
        public string Message { get; set; }
        public bool IsSystemMessage { get; set; }

        public IDtoShort MapToShortDto()
        {
            var shortDto = new ChatMessageDtoShort
            {
                S = Source,
                Ism = IsSystemMessage,
                M = Message,
                Sc = SourceColour,
                T = Timestamp
            };
            return shortDto;
        }
    }
}
