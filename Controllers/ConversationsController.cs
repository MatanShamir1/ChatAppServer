#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChatApp.Data;

namespace ChatApp.Controllers
{

    [ApiController]
    [Route("api")]
    public class ConversationsController : Controller
    {
        public static string getTime()
        {
            DateTime date = DateTime.Now;
            return date.ToString("o");
        }
        private readonly ChatAppContext _context;

        public ConversationsController(ChatAppContext context)
        {
            _context = context;

            User u = new User()
            {
                Username = "Matan",
                Nickname = "Tani",
                Conversations = new List<Conversation>(),
                Password = "aaa"
            };
            Message msg = new Message() { Content = "Matan:Hello", Time = getTime() };

            Conversation conv = new Conversation() { RemoteUser = null, Messages = new List<Message>() { msg }, RemoteUserId = 1, User = u };
            RemoteUser ru = new RemoteUser() { Username = "Coral", Nickname = "Corali", Conversation = conv, Server = "remote", ConversationId = 1 };
            u.Conversations.Add(conv);
            conv.RemoteUser = ru;
            _context.Add(msg);
            _context.Add(u);
            _context.Add(ru);
            _context.Add(conv);


            //_context.SaveChanges();
        }

        // GET: /contacts + /contacts/:id
        [HttpGet("contacts/{id?}")]
        public async Task<IActionResult> GettAllContacts(string? id)
        {
            string name = "Matan";
            //string name = HttpContext.Session.GetString("username");

            if (id != null)
            {
                var q = from conversations in _context.Conversations
                        where conversations.RemoteUser.Username == id && conversations.User.Username == name
                        select conversations.RemoteUser;

                RemoteUser remoteUser = q.First();

                Message lastMessage = getLastMessage(remoteUser, name);
                string content = getContentFromMessage(lastMessage);

                return Json(new _User()
                {
                    Id = remoteUser.Username,
                    Name = remoteUser.Nickname,
                    Server = remoteUser.Server,
                    LastDate = lastMessage.Time,
                    Last = content
                });
            }


            //string name = HttpContext.Session.GetString("username");

            var contacts = from conversations in _context.Conversations
                           where conversations.User.Username == name
                           select conversations.RemoteUser.Nickname;

            return Json(contacts);
            //return View(await _context.Conversations.ToListAsync());
        }



        [HttpPost]
        public async Task<IActionResult> Index([Bind("UserName, NickName, Server")] RemoteUser remoteUser)
        {
            if (ModelState.IsValid)
            {
                remoteUser.Id = _context.RemoteUsers.Max(x => x.Id) + 1;
                remoteUser.Conversation = new Conversation();
                _context.RemoteUsers.Add(remoteUser);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                //return View("good");
                return Json(new EmptyResult());
            }
            return View("not");
        }


        // PUT: /contacts/id
        [HttpPut, ActionName("contacts")]
        public async Task<IActionResult> Put(string contact, [Bind("UserName, NickName, Server")] RemoteUser ru)
        {
            if (ru == null)
            {
                return Json(new EmptyResult());
            }

            RemoteUser remoteUser = (RemoteUser)from remote in _context.RemoteUsers
                                                where remote.Username == contact && remote.Server == ru.Server
                                                select remote;
            remoteUser.Username = ru.Username;
            remoteUser.Nickname = ru.Nickname;
            return NoContent();    //204
        }


        // DELETE: /contacts/id
        [HttpDelete, ActionName("contacts")]
        public async Task<IActionResult> Delete(string contact)
        {
            if (contact == null)
            {
                return Json(new EmptyResult());
            }

            string name = HttpContext.Session.GetString("username");

            var remoteUser = from conversations in _context.Conversations
                             where conversations.RemoteUser.Username == contact && conversations.User.Username == name
                             select conversations.RemoteUser;

            _context.RemoteUsers.Remove(remoteUser.First());
            _context.SaveChanges();
            return NoContent();    //204
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