using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Core.Entities;

namespace PhoneBookApp.Infrastructure.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        public DbSet<UserLogin> UserLogin { get; set; }

    }
}
