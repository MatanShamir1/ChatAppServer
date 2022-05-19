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
using ChatApp.Services;

namespace ChatApp.Controllers
{

    [ApiController]
    [Route("api/contacts/{id}/messages")]
    public class MessagesController : Controller
    {
        private readonly MessagesService _service;

        public MessagesController(ChatAppContext context)
        {
            _service = new MessagesService(context);
        }

        // GET: Messages
        [HttpGet("{id2?}")]
        public async Task<IActionResult> GetAllMessages(string id, int id2)
        {
            //string name = "12345";
            string name = HttpContext.Session.GetString("username");

            if (id2 != 0)
            {
                _Message specific = await _service.GetSpecificMessage(name,id,id2);

                return Json(specific);
            }

            List<_Message> messagesList = await _service.GetAllMessages(name,id,id2);

            if(messagesList == null)
            {
                return Json("empty");
            }

            return Json(messagesList);
        }

        // POST: Messages
        [HttpPost]
        public async Task<IActionResult> AddNewMessage(string id,[Bind("content")] Message message)
        {
            //string username = "12345";
            string name = HttpContext.Session.GetString("username");

            string status = await _service.AddNewMessage(name, id, message, "first");
            
            if(status == "201")
            {
                return StatusCode(201);
            }
            else
            {
                return BadRequest();
            }
        }
        
        
        [HttpDelete("{id2?}")]
        public async Task<IActionResult> RemoveMessage(string id, int id2)
        {
            if (id2 == 0)
            {
                return BadRequest();
            }

            string status = await _service.DeleteMessage(id, id2);

            if (status == "204")
            {
                return StatusCode(204);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id2?}")]
        public async Task<IActionResult> UpdateMessage(string id, int id2, [Bind("content")] _Message mess)
        {
            if (id2 == 0)
            {
                return BadRequest();
            }
            string status = await _service.UpdateMessage(id, id2, mess);
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




