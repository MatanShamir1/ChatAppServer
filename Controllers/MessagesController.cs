#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using ChatApp.Models;
using ChatApp.Data;

namespace ChatApp.Controllers
{

    [ApiController]
    // ???????????????????????????????????????????
    [Route("api/contacts/{id}/messages")]
    public class MessagesController : Controller
    {
        private readonly ChatAppContext _context;

        public MessagesController(ChatAppContext context)
        {
            _context = context;
        }

        // GET: Messages
        [HttpGet("{id2?}")]
        public async Task<IActionResult> GetAllMessages(string id, int id2)
        {
            string name = "12345";
            //string name = HttpContext.Session.GetString("username");

            if (id2 != 0)
            {
                var q = from message in _context.Messages
                        where message.Id == id2
                        select message;

                Message mess = q.First();

                bool sent;

                if (getSenderFromMessage(mess) == id) { sent = false; } else { sent = true; }

                return Json(new _Message() { Content = getContentFromMessage(mess), Created = getTime(), Id = mess.Id, Sent = sent });
            }
            //var messages = await _context.Messages.ToListAsync();
            //var qu = from conversations in _context.Conversations
              //       where conversations.User.Username == "12345" && conversations.RemoteUser.Username == id
                //     select conversations.Messages.ToList();

            var query = _context.Conversations.Include(m => m.Messages).Where(c => c.User.Username == name && c.RemoteUser.Username == id).ToList();

            if (!query.Any())
            {
                return Json("empty");
            }

            List<Message> messages = query.First().Messages;

            if (!messages.Any())
            {
                return Json("empty");
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

            return Json(messagesList);
        }

        // POST: Messages
        [HttpPost]
        public async Task<IActionResult> SetMessageContent(string id,[Bind("content")] Message message)
        {
            string username = "12345";
            //string username = HttpContext.Session.GetString("username");

            var conversation = from conv in _context.Conversations.Include(c => c.Messages)
                               where conv.User.Username == username && conv.RemoteUser.Username == id
                               select conv;

            Conversation conver = conversation.FirstOrDefault();

            string newContent = username + ":" + message.Content;

            message.Content = newContent;

            message.Time = getTime();

            conver.Messages.Add(message);

            _context.SaveChanges();

            return StatusCode(201);    // 201
        }

        // GET: Messages/Details/5
        [HttpPut, ActionName("messages")]
        public async Task<IActionResult> RemoveMessage(string id, int? id2, [Bind("content")] string content)
        {
            Message mess = (Message)from message in _context.Messages
                                    where message.Id == id2
                                    select message;
            _context.Messages.Remove(mess);
            _context.SaveChanges();
            return NoContent();    //204
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




