using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class Rating
    {
        public int Id { get; set; }


        [Range(1, 5)]
        public int Rate { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Name { get; set; }

        public string? Time { get; set; }

    }
}