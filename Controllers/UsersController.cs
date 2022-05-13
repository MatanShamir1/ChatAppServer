#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Register([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                var isTakenUserName = from userName in _context.Users.Where(m => m.Username == user.Username) select userName;
                if (isTakenUserName.Any())
                {
                    return Json("already register");
                }
                else
                {
                    Regex password = new Regex(@"[0-9]+");
                    
                    Regex name = new Regex(@"^[a-zA-Z]+$");
                   
                    if (name.IsMatch(user.Username))
                    {
                        if (password.IsMatch(user.Password))
                        {
                            if (name.IsMatch(user.Nickname))
                            {
                                //HttpContext.Session.SetString("username", isTakenUserName.First().Username);
                                _context.Add(user);
                                _context.SaveChangesAsync();
                                return Json("yes");
                            }
                            else { return Json("no"); }
                        }
                        else
                        {
                            return  Json("no");
                        }
                    }
                    else
                    {
                        return Json("no");
                    }
                }
            }
            return View(user);
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
                if (isRegistered.Any())
                {
                    // we save info and when the user refreshes we know its him
                    HttpContext.Session.SetString("username", isRegistered.First().Username);
                    // rediret with react
                    return Json("yes");
                }
                else
                {
                    return Json("no");
                }


            }
            return Ok(200);
            
        }

    }
}