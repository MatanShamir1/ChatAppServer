namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public Conversation Conversation { get; set; }

        public string Time { get; set; }

    }
}
