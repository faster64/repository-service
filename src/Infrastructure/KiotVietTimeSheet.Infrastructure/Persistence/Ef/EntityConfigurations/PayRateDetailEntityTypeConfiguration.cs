using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayRateDetailEntityTypeConfiguration : IEntityTypeConfiguration<PayRateDetail>
    {
        public void Configure(EntityTypeBuilder<PayRateDetail> builder)
        {
            builder.ToTable("PayRateDetail")
                .HasKey(t => t.Id);

            builder.ToTable("PayRateDetail")
                .HasIndex(p => p.PayRateId)
                .HasName("NonClusteredIndex-PayRateDetail-PayRateId");

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.PayRateId)
                .IsRequired();

            builder.Property(t => t.RuleType)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.RuleValue)
                .IsRequired()
                .HasColumnType("NTEXT");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");
        }
    }
}
