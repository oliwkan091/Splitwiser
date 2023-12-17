using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Splitwiser.Models.PaymentMember
{
    public class PaymentMemberEntityConfiguration : IEntityTypeConfiguration<PaymentMemberEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentMemberEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.AddDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
