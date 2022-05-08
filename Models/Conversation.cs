using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public User User { get; set; }
        //[ForeignKey("RemoteUser")]
        public RemoteUser RemoteUser { get; set; }
        public List<Message> Messages { get; set; }
    }
}
