using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<_User> GetContactById(string remote_name, string local_name) {
            var q = from conversations in _context.Conversations
                    where conversations.RemoteUser.Username == remote_name && conversations.User.Username == local_name
                    select conversations.RemoteUser;

            RemoteUser remoteUser = q.FirstOrDefault();

            Message lastMessage = getLastMessage(remoteUser, local_name);
            string content = "";
            string lastDate = "";
            if (lastMessage != null)
            {
                content = getContentFromMessage(lastMessage);
                lastDate = lastMessage.Time;
            }
           
            return new _User()
            {
                Id = remoteUser.Username,
                Name = remoteUser.Nickname,
                Server = remoteUser.Server,
                LastDate = lastDate,
                Last = content
            };
        }

        public async Task<List<_User>> GetContacts(string name)
        {
            var contacts = _context.Conversations.Include(m => m.RemoteUser).Where(c => c.User.Username == name).ToList();

            List<_User> users = new List<_User>();

            foreach (var contact in contacts)
            {
                users.Add(await this.GetContactById(contact.RemoteUser.Username, name));
            }

            return users;
        }

        public async Task<string> InviteContact(_Invitation invitation)
        {
            var q = from users in _context.Users
                    where users.Username == invitation.To
                    select users;
            if (!q.Any())
            {
                //send back a bad request, user doesn't exist!
                return "bad";
            }
            //in this case, user exists, add "from" to "to"'s contacts list.

            //now, check if "from" somehow already exists. this is an edge case, but needed.
            var alreadyExists = from conver in _context.Conversations
                                where conver.User.Username == invitation.To && conver.RemoteUser.Username == invitation.From
                                select conver;

            if (alreadyExists.Any())
            {
                return "201";
            }

            User to = q.First();
            Conversation conversation = new Conversation() { User = to, Messages = new List<Message>() };
            RemoteUser From = new RemoteUser()
            {
                //we dont have the other user's nickname
                Conversation = conversation,
                Username = invitation.From,
                Server = invitation.Server,
                ConversationId = conversation.Id
            };
            conversation.RemoteUser = From;
            conversation.RemoteUserId = From.Id;
            await _context.RemoteUsers.AddAsync(From);
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return "201";
        }

        public async Task<string> CreateContact(_User initialRemoteUser, string name)
        {
            var user = from users in _context.Users.Include(m => m.Conversations)
                       where users.Username == name
                       select users;
            if (!user.Any())
            {
                return "bad";
            }
            User current = user.First();
            Conversation conversation = new Conversation() { User = current, Messages = new List<Message>() };
            RemoteUser remoteUser = new RemoteUser()
            {
                Conversation = conversation,
                Nickname = initialRemoteUser.Name,
                Username = initialRemoteUser.Id,
                Server = initialRemoteUser.Server,
                ConversationId = conversation.Id
            };
            conversation.RemoteUser = remoteUser;
            conversation.RemoteUserId = remoteUser.Id;
            _context.RemoteUsers.Add(remoteUser);
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return "201";
        }

        public async Task<string> UpdateContact(_RemoteUser ru, string name, string id)
        {
            var q = from conversations in _context.Conversations.Include(c => c.RemoteUser)
                    where conversations.RemoteUser.Username == id && conversations.User.Username == name
                    select conversations.RemoteUser;

            if (!q.Any())
            {
                return "bad";
            }
            RemoteUser remoteUser = q.First();
            remoteUser.Server = ru.Server;
            remoteUser.Nickname = ru.Name;
            await _context.SaveChangesAsync();
            return "204";
        }

        public async Task<string> DeleteContact(string id, string name)
        {
            var q = from conversations in _context.Conversations.Include(c => c.RemoteUser)
                    where conversations.RemoteUser.Username == id && conversations.User.Username == name
                    select conversations.RemoteUser;
            if (!q.Any())
            {
                return "bad";
            }
            RemoteUser remoteUser = q.First();
            _context.RemoteUsers.Remove(remoteUser);
            await _context.SaveChangesAsync();
            return "204";
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
