using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Splitwiser.Models.PaymentInGroup
{
    public class PaymentInGroupEntityConfiguration : IEntityTypeConfiguration<PaymentInGroupEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentInGroupEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
