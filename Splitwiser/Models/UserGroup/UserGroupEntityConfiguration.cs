using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Splitwiser.Models.UserGroup
{
    public class UserGroupEntityConfiguration : IEntityTypeConfiguration<UserGroupEntity>
    {
        public void Configure(EntityTypeBuilder<UserGroupEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.AddDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
