using Microsoft.EntityFrameworkCore;
using Splitwiser.Models.Group;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.UserGroup;

namespace Splitwiser.EntityFramework
{
    public class SplitwiserDbContext : DbContext
    {
        public SplitwiserDbContext(DbContextOptions<SplitwiserDbContext> options) : base(options) { }

        public DbSet<GroupEntity> Groups { get; set; }

        public DbSet<UserGroupEntity> UserGroups { get; set; }
        public DbSet<GroupPaymentHistoryEntity> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplyConfiguration(new UserGroupEntityConfiguration())
                .ApplyConfiguration(new GroupEntityConfiguration())
                .ApplyConfiguration(new GroupPaymentHistoryEntityConfiguration())
                ;
        }
    }
}
