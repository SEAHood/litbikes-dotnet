using System;
using System.Collections.Generic;
using System.Text;

namespace LitBikes.Model.Dtos
{
    public class ChatMessageDto
    {
        public long Timestamp;
        public string Source;
        public string SourceColour; // in rgba(0,0,0,0) format
        public string Message;
        public bool IsSystemMessage;

        public ChatMessageDto(string source, string sourceColour, string message, bool isSystemMessage)
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Source = source;
            SourceColour = sourceColour;
            Message = message;
            IsSystemMessage = isSystemMessage;
        }
    }
}
