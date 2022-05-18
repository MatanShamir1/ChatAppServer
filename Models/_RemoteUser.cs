namespace ChatApp.Models
{
    public class _RemoteUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public Conversation? Conversation { get; set; }
        public string? Server { get; set; }
    }
}
