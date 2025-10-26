using Microsoft.EntityFrameworkCore;
using PhoneBook.Core.Entities;
using PhoneBookApp.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PhoneBookApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Person> People => Set<Person>();
        public DbSet<PhoneNumber> PhoneNumbers => Set<PhoneNumber>();
        public DbSet<Admin> Admins { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Category + Locations
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "داخلي" });

            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, Name = "المعادي", CategoryId = 1 },
                new Location { Id = 2, Name = "البواخر", CategoryId = 1 },
                new Location { Id = 3, Name = "فنادق السخنة", CategoryId = 1 },
                new Location { Id = 4, Name = "فنادق الغردقة", CategoryId = 1 },
                new Location { Id = 5, Name = "المؤسسة", CategoryId = 1 }
            );

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin"); 
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.User_Name).HasColumnName("user_name").IsRequired().HasMaxLength(100);
            });
        }
    }
}
