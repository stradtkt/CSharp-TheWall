using Microsoft.EntityFrameworkCore;
 
namespace Wall.Models
{
    public class WallContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public WallContext(DbContextOptions<WallContext> options) : base(options) { }

        public DbSet<User> users {get;set;}
        public DbSet<Message> messages {get;set;}
        public DbSet<Comment> comments {get;set;}
    }
}