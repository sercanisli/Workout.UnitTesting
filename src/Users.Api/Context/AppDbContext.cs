using Microsoft.EntityFrameworkCore;
using Users.Api.Models;

namespace Users.Api.Context
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
