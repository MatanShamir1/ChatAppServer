#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;

namespace ChatApp.Data
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext (DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        public DbSet<ChatApp.Models.Rating> Ratings { get; set; }
        public DbSet<ChatApp.Models.User> Users { get; set; }
        public DbSet<ChatApp.Models.Message> Messages { get; set; }
        public DbSet<ChatApp.Models.Conversation> Conversations { get; set; }

        public DbSet<ChatApp.Models.RemoteUser> RemoteUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conversation>()
                .HasOne(a => a.RemoteUser)
                .WithOne(b => b.Conversation)
                .HasForeignKey<RemoteUser>(a => a.Username);
        }
    }
}
