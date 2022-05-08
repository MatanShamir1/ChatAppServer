using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class User
    { 
        [Key]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public List<Conversation> Conversation { get; set; }

    }
}
