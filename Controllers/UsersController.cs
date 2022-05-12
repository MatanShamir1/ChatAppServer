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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Password,NickName")] User user)
        {
            if (ModelState.IsValid)
            {

                var isTakenUserName = from userName in _context.Users.Where(m => m.Username == user.Username) select userName;
                if (isTakenUserName.Any())
                {
                    return View("Error");
                }
                else
                {
                    HttpContext.Session.SetString("username", isTakenUserName.First().Username);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(user);
        }


        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Login()
        {
            User user = new User(); 
            
            if (ModelState.IsValid)
            {

                var isRegistered = _context.Users.Where(m => m.Username == user.Username && m.Password == user.Password);
                if (isRegistered.Any())
                {
                    // we save info and when the user refreshes we know its him
                    HttpContext.Session.SetString("username", isRegistered.First().Username);
                    // rediret with react
                    return View("yes");
                }
                else
                {
                    return View("no");
                }


            }

            return Ok(200);
            
        }

    }
}