using OpenAI_API.Chat;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class ChatViewModel
    {
        [Required]
        public string? UserInput { get; set; }

        public string? BotResponse { get; set; }

        public List<ChatMessage>? ChatHistoryMessages { get; set; }
    }
}
