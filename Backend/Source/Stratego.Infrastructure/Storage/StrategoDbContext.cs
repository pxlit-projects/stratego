using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stratego.Domain;

namespace Stratego.Infrastructure.Storage
{
    //DO NOT TOUCH THIS FILE!!
    public class StrategoDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Friendship> Friendships { get; set; }

        public StrategoDbContext()
        {
        }

        public StrategoDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StrategoDb;Integrated Security=True";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Friendship>().HasKey(f => new {f.Friend1Id, f.Friend2Id});
        }
    }
}
