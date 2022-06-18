#nullable disable

using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Services;
using ChatApp.Hubs;

namespace ChatApp.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {

        private readonly UsersService _service;

        public UsersController(ChatAppContext context)
        {
            _service = new UsersService(context);

        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                string status = await _service.RegisterNewUser(user);
                if(status == "201")
                {
                    return StatusCode(201);
                }
                else
                {
                    return StatusCode(202);
                }  
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddToken([FromBody] Token token)
        {
            if (ModelState.IsValid)
            {
                string name = HttpContext.Session.GetString("username");
                if (name == null)
                {
                    return StatusCode(400);
                }
                else
                {
                    AndroidHub.tokenDic.Add(name, token.newtoken);
                    return StatusCode(201);
                }


            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteToken()
        {
            if (ModelState.IsValid)
            {
                string name = HttpContext.Session.GetString("username");
                if (name == null)
                {
                    return StatusCode(400);
                }
                else
                {
                   if(AndroidHub.tokenDic[name] != null)
                    {
                        AndroidHub.tokenDic[name] = null;
                    }
                    return StatusCode(201);
                }


            }
            return BadRequest();
        }
        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //public IActionResult Login([FromBody]User user)
        public async Task<IActionResult> Login([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                User isRegistered = await _service.UserLogin(user);
                if (isRegistered != null)
                {
                    // we save info and when the user refreshes we know its him
                    HttpContext.Session.SetString("username", isRegistered.Username);

                    // rediret with react
                    return StatusCode(201);
                }
                else
                {
                    return StatusCode(202);
                }
            }
            return BadRequest();
        }

        [HttpGet]
        //public IActionResult Login([FromBody]User user)
        public async Task<IActionResult> IsExists()
        {
            if (ModelState.IsValid)
            {
                string name = HttpContext.Session.GetString("username");
                if (name != null)
                {
                    return Json(name);
                }
                else
                {
                    return Json("");
                }
            }
            return BadRequest();
        }

        [HttpPost]
        //public IActionResult Login([FromBody]User user)
        public async Task<IActionResult> LogOut()
        {
            if (ModelState.IsValid)
            {
                string name = HttpContext.Session.GetString("username");
                if (name == null)
                {
                    return Ok();
                }
                HttpContext.Session.Remove("username");
                return Ok();
            }
            return BadRequest();
        }
    }
}