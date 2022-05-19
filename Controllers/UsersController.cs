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
using ChatApp.Data;
using System.Text.RegularExpressions;

namespace ChatApp.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {

        private readonly ChatAppContext _context;

        public UsersController(ChatAppContext context)
        {
            _context = context;

        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                var isTakenUserName = from username in _context.Users.Where(m => m.Username == user.Username) select username;
                if (isTakenUserName.Any())
                {
                    return BadRequest();
                }
                else
                { 
                    _context.Add(user);
                    await _context.SaveChangesAsync();
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
        public IActionResult Login([FromBody]User user)
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _context.Users.Where(m => m.Username == user.Username && m.Password == user.Password);
                if(isRegistered.Any())
                {
                    // we save info and when the user refreshes we know its him
                    HttpContext.Session.SetString("username", isRegistered.First().Username);
                
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