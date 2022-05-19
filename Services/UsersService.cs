using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class UsersService
    {
        private readonly ChatAppContext _context;
        public UsersService(ChatAppContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterNewUser(User user)
        {
            var isTakenUserName = from username in _context.Users.Where(m => m.Username == user.Username) select username;
            if (isTakenUserName.Any())
            {
                return "bad";
            }
            else
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return "201";

            }
        }

        public async Task<User> UserLogin(User user)
        {
            var isRegistered = _context.Users.Where(m => m.Username == user.Username && m.Password == user.Password);
            if (isRegistered.Any())
            {
                return isRegistered.First();
            }
            else
            {
                return null;
            }
        }

    }
}
