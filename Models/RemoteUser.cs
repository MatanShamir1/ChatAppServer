using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class RemoteUser
    {
        public int Id { get; set; }
        public string? Nickname { get; set; }
        public string Username { get; set; }
        public int ConversationId { get; set; }
        public Conversation? Conversation { get; set; }
        public string? Server { get; set; }
    }
}
