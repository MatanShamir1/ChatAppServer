using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class RemoteUser
    {
        [Key]
        public string Username { get; set; }
        public Conversation Conversation { get; set; }
        public string Server { get; set; }
    }
}
