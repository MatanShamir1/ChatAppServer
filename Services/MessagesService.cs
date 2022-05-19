using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class MessagesService
    {
        private readonly ChatAppContext _context;
        public MessagesService(ChatAppContext context)
        {
            _context = context;
        }

        public async Task<_Message> GetSpecificMessage(string name, string id, int id2)
        {
            var q = from message in _context.Messages
                    where message.Id == id2
                    select message;

            Message mess = q.First();

            bool sent;

            if (getSenderFromMessage(mess) == id) { sent = false; } else { sent = true; }
            return new _Message() { Content = getContentFromMessage(mess), Created = getTime(), Id = mess.Id, Sent = sent };
        }

        public async Task<List<_Message>> GetAllMessages(string name, string id, int id2)
        {
            var query = _context.Conversations.Include(m => m.Messages).Where(c => c.User.Username == name && c.RemoteUser.Username == id).ToList();

            if (!query.Any())
            {
                return null;
            }

            List<Message> messages = query.First().Messages;

            if (!messages.Any())
            {
                return null;
            }

            var messagesList = new List<_Message>();

            foreach (Message message in messages)
            {
                string sender = getSenderFromMessage(message);
                string content = getContentFromMessage(message);
                string time = message.Time;
                int Id = message.Id;

                bool sent;

                if (sender == name) { sent = true; } else { sent = false; }

                messagesList.Add(new _Message() { Content = content, Id = Id, Sent = sent, Created = time });
            }

            return messagesList;
        }

        public async Task<string> AddNewMessage(string name, string id, Message message, string sender)
        {
            var conversation = from conv in _context.Conversations.Include(c => c.Messages)
                               where conv.User.Username == name && conv.RemoteUser.Username == id
                               select conv;

            if (!conversation.Any())
            {
                return "bad";
            }
            Conversation conver = conversation.FirstOrDefault();
            string name_of_sender = name;
            if (sender == "second")
            {
                name_of_sender = id;
            }
            string newContent = name_of_sender + ":" + message.Content;

            message.Content = newContent;

            message.Time = getTime();

            //check if number of messaged is zero, and then send an invitation
            //and also if its another server and then send transfer.

            //no need to add to messages database; entity frmework adds it alone because of their relation.
            conver.Messages.Add(message);

            await _context.SaveChangesAsync();

            return "201";
        }

        public async Task<string> DeleteMessage(string id, int id2)
        {
            var q = from message in _context.Messages
                    where message.Id == id2
                    select message;
            _context.Messages.Remove(q.First());
            await _context.SaveChangesAsync();
            return "204";
        }

        public async Task<string> UpdateMessage(string id, int id2, _Message mess)
        {
            var q = from message in _context.Messages
                    where message.Id == id2
                    select message;
            Message toUpdate = q.First();
            toUpdate.Content = mess.Content;
            await _context.SaveChangesAsync();
            return "204";
        }

        public static string getTime()
        {
            DateTime date = DateTime.Now;
            return date.ToString("o");
        }
        private string getContentFromMessage(Message message)
        {
            int start = message.Content.ToString().IndexOf(":") + 1;
            return message.Content.ToString().Substring(start);
        }

        private string getSenderFromMessage(Message message)
        {
            int end = message.Content.ToString().IndexOf(":");
            return message.Content.ToString().Substring(0, end);
        }
    }
}
