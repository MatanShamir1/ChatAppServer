using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class ConversationsService
    {
        private readonly ChatAppContext _context;
        public ConversationsService(ChatAppContext context)
        {
            _context = context;
        }

        public _User GetContactById(string remote_name, string local_name) {
            var q = from conversations in _context.Conversations
                    where conversations.RemoteUser.Username == remote_name && conversations.User.Username == local_name
                    select conversations.RemoteUser;

            RemoteUser remoteUser = q.FirstOrDefault();

            Message lastMessage = getLastMessage(remoteUser, local_name);
            string content = getContentFromMessage(lastMessage);
            return new _User()
            {
                Id = remoteUser.Username,
                Name = remoteUser.Nickname,
                Server = remoteUser.Server,
                LastDate = lastMessage.Time,
                Last = content
            };
        }

        private Message getLastMessage(RemoteUser ru, string name)
        {
            var q = from conv in _context.Conversations.Include(m => m.Messages)
                    where conv.User.Username == name && conv.RemoteUser == ru
                    select conv;

            Conversation c = q.First();
            return c.Messages.OrderByDescending(m => m.Id).FirstOrDefault();
        }
        private string getContentFromMessage(Message message)
        {
            int start = message.Content.ToString().IndexOf(":") + 1;
            return message.Content.ToString().Substring(start);
        }

    }
}
