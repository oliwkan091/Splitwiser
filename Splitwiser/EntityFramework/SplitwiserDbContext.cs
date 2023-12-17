using Microsoft.EntityFrameworkCore;
using Splitwiser.Models.Group;
using Splitwiser.Models.GroupPaymentHistory;
using Splitwiser.Models.PaymentInGroup;
using Splitwiser.Models.PaymentMember;
using Splitwiser.Models.UserGroup;

namespace Splitwiser.EntityFramework
{
    public class SplitwiserDbContext : DbContext
    {
        public SplitwiserDbContext(DbContextOptions<SplitwiserDbContext> options) : base(options) { }

        public DbSet<GroupEntity> Groups { get; set; }

        public DbSet<UserGroupEntity> UserGroups { get; set; }
        public DbSet<GroupPaymentHistoryEntity> Payments { get; set; }
        public DbSet<PaymentMemberEntity> PaymentMembers { get; set; }
        public DbSet<PaymentInGroupEntity> PaymentInGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplyConfiguration(new UserGroupEntityConfiguration())
                .ApplyConfiguration(new GroupEntityConfiguration())
                .ApplyConfiguration(new GroupPaymentHistoryEntityConfiguration())
                .ApplyConfiguration(new PaymentMemberEntityConfiguration())
                .ApplyConfiguration(new PaymentInGroupEntityConfiguration())
                ;
        }
    }
}
