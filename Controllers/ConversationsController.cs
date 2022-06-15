#nullable disable
using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using Message = FirebaseAdmin.Messaging.Message;
using my_Message = ChatApp.Models.Message;

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
        private readonly ConversationsService _service;
        private readonly IHubContext<ChatHub> _hubContext;
        public ConversationsController(ChatAppContext context , IHubContext<ChatHub> hubContext)
        {
            _service = new ConversationsService(context);
    
            _hubContext = hubContext;


            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            }
            //User u = new User()
            //{
            //    Username = "12345",
            //    Nickname = "Tani",
            //    Conversations = new List<Conversation>(),
            //    Password = "aaa"
            //};
            //my_Message msg = new my_Message() { Content = "12345:Hello", Time = getTime() };

            //Conversation conv = new Conversation() { RemoteUser = null, Messages = new List<my_Message>() { msg }, RemoteUserId = 1, User = u };
            //RemoteUser ru = new RemoteUser() { Username = "Coral", Nickname = "Corali", Conversation = conv, Server = "remote", ConversationId = 1 };
            //u.Conversations.Add(conv);
            //conv.RemoteUser = ru;
            //context.Add(msg);
            //context.Add(u);
            //context.Add(ru);
            //context.Add(conv);


            //context.SaveChanges();
        }
    public async Task SendToAll(string user ,string from){
            if (ChatHub.UserAndConnect.ContainsKey(user))
            {
                _User remoteUser = await _service.GetContactById(user, from);
                if (ChatHub.UserAndConnect[user] != null)
                {
                    await _hubContext.Clients.Client(ChatHub.UserAndConnect[user]).SendAsync("RecieveMessage", from, remoteUser.Server);
                }
            }
            if (AndroidHub.tokenDic[user] != null)
            {
                //need to handle sending to firebase right here.

           
                var token = AndroidHub.tokenDic[user];


                var message = new Message()
                {
                    Data = new Dictionary<string, string>()
                    {
                        { "key", "value" },
                    },
                    Token = token,
                    Notification = new Notification()
                    {
                        // need to send the message
                        Title = "You have new Message!",
                        Body = "You received a message from " + from
                    }

                };
                string response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
                Console.WriteLine("message:" + response);

            }
                
            
     }
        
        [HttpGet("contacts/{id?}")]
        public async Task<IActionResult> GetAllContacts(string? id)
        {
            string name = HttpContext.Session.GetString("username");

            if (id != null)
            {
                _User remoteUser = await _service.GetContactById(id,name);

                return Json(remoteUser);
            }

            List<_User> users = await _service.GetContacts(name);

            return Json(users);
        }

        [HttpPost ("invitations")]
        public async Task<IActionResult> Invitation([FromBody] _Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                string status = await _service.InviteContact(invitation);
                if(status == "bad")
                {
                    return BadRequest();
                }
                else
                {
                    return StatusCode(201);
                }
            }
            return BadRequest();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] _Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                string status = await _service.TransferToUser(transfer);
                if (status == "bad")
                {
                    return BadRequest();
                }
                else
                {
                    await this.SendToAll(transfer.To , transfer.From);
                    return StatusCode(201);
                }
            }
            return BadRequest();
        }

        [HttpPost ("contacts/{id?}")]
        public async Task<IActionResult> CreateContact([FromBody] _User initialRemoteUser)
        {
            if (ModelState.IsValid)
            {
                string name = HttpContext.Session.GetString("username");
                string status = await _service.CreateContact(initialRemoteUser, name);
                if(status == "201")
                {
                    return StatusCode(201);
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }


        // PUT: /contacts/id
        [HttpPut("contacts/{id?}")]
        public async Task<IActionResult> Put(string id, [Bind("Name, Server")] _RemoteUser ru)
        {
            if (id == null)
            {
                return BadRequest();
            }

            string name = HttpContext.Session.GetString("username");

            string status = await _service.UpdateContact(ru, name, id);

            if (status == "204")
            {
                return StatusCode(204);
            }
            else
            {
                return BadRequest();
            }

        }


        // DELETE: /contacts/id
        [HttpDelete("contacts/{id?}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            string name = HttpContext.Session.GetString("username");
            string status = await _service.DeleteContact(id, name);

            if (status == "204")
            {
                return StatusCode(204);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}