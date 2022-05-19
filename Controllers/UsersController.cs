#nullable disable

using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using ChatApp.Data;
using ChatApp.Services;

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
                    return BadRequest();
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
                    return BadRequest();
                }
            }
            return BadRequest();
        }
    }
}