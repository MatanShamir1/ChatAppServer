using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
       public static Dictionary<string, string> UserAndConnect = new Dictionary<string, string>();

        
        public async Task JoinRoom (string user)
        {
            
            UserAndConnect.Add(user, Context.ConnectionId);
        }
        public async Task Remove(string user)
        {

            UserAndConnect.Remove(user);
        }
    }
}
