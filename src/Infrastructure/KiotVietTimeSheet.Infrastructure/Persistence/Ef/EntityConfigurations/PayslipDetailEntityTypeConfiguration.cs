using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayslipDetailEntityTypeConfiguration : IEntityTypeConfiguration<PayslipDetail>
    {
        public void Configure(EntityTypeBuilder<PayslipDetail> builder)
        {
            builder.ToTable("PayslipDetail")
                .HasKey(t => t.Id);

            builder.ToTable("PayslipDetail")
                .HasIndex(p => p.PayslipId)
                .HasName("NonClusteredIndex-PayslipDetail-PayslipId");

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.PayslipId)
                .IsRequired();

            builder.Property(t => t.RuleType)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.RuleValue)
                .IsRequired()
                .HasColumnType("NTEXT");

            builder.Property(t => t.RuleParam)
                .IsRequired()
                .HasColumnType("NTEXT");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");
        }
    }
}
