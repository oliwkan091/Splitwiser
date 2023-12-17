using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Splitwiser.Models.UserEntity;

namespace Splitwiser.EntityFramework
{
    public class DbSplitwiserAuthContext : IdentityDbContext<UserEntity>
    {
        public DbSplitwiserAuthContext(DbContextOptions<DbSplitwiserAuthContext> options) : base(options) { }

        public DbSet<UserEntity> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
