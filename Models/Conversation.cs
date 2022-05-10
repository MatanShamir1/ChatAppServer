using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public User User { get; set; }

        public int RemoteUserId { get; set; }
        public RemoteUser RemoteUser { get; set; }
        public List<Message> Messages { get; set; }
    }
}
