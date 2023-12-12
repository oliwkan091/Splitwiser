using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Splitwiser.Models;

namespace Splitwiser.EntityFramework
{
    public class DbSplitwiserAuthContext : IdentityDbContext<UserModel>
    {
        public DbSplitwiserAuthContext(DbContextOptions<DbSplitwiserAuthContext> options) : base(options) { }

        public DbSet<UserModel> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
