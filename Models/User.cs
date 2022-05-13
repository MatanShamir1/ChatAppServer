using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class User
    { 
        [Key]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? Nickname { get; set; }

        public List<Conversation>? Conversations { get; set; }

    }
}
