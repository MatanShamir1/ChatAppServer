#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChatApp.Data;
using ChatApp.Services;

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
        private readonly ConversationsService _service;

        public ConversationsController(ChatAppContext context)
        {
            _service = new ConversationsService(context);
            _context = context;

            User u = new User()
            {
                Username = "12345",
                Nickname = "Tani",
                Conversations = new List<Conversation>(),
                Password = "aaa"
            };
            Message msg = new Message() { Content = "12345:Hello", Time = getTime() };

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
            string name = "12345";
            //string name = HttpContext.Session.GetString("username");

            if (id != null)
            {
                _User remoteUser = _service.GetContactById(id,name);

                return Json(remoteUser);
            }


            /*var contacts = from conversations in _context.Conversations
                           where conversations.User.Username == name
                           select conversations.RemoteUserToList;*/
            var contacts = _context.Conversations.Include(m => m.RemoteUser).Where(c => c.User.Username == name).ToList();

            List<_User> users = new List<_User>();

            foreach (var contact in contacts)
            {
                users.Add(_service.GetContactById(contact.RemoteUser.Username, name));
            }

            return Json(users);
            //return View(await _context.Conversations.ToListAsync());
        }



        [HttpPost ("contacts/{id?}")]
        public IActionResult Index([FromBody] RemoteUser remoteUser)
        {
            if (ModelState.IsValid)
            {
                string name = "Matan";
                //string name = HttpContext.Session.GetString("username");
                //remoteUser.Id = _context.RemoteUsers.Max(x => x.Id) + 1;
                var user = from users in _context.Users
                           where users.Username == "12345"
                           select users;
                User current = user.First();
                Conversation conversation = new Conversation() { User = current, RemoteUser = remoteUser, Messages = new List<Message>(), RemoteUserId = remoteUser.Id};
                remoteUser.Conversation = conversation;
                _context.RemoteUsers.Add(remoteUser);
                _context.Conversations.Add(conversation);
                 _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                //return View("good");
                return StatusCode(201);
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