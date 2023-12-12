using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Splitwiser.Models.GroupPaymentHistory
{
    public class GroupPaymentHistoryEntityConfiguration : IEntityTypeConfiguration<GroupPaymentHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<GroupPaymentHistoryEntity> builder)
        {
			builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.AddDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
