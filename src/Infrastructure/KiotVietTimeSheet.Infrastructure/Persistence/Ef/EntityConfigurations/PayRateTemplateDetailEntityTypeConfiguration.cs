using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayRateTemplateDetailEntityTypeConfiguration : IEntityTypeConfiguration<PayRateTemplateDetail>
    {
        public void Configure(EntityTypeBuilder<PayRateTemplateDetail> builder)
        {
            builder.ToTable("PayRateTemplateDetail")
                .HasKey(t => t.Id);

            builder.ToTable("PayRateTemplateDetail")
                .HasIndex(p => p.PayRateTemplateId)
                .HasName("NonClusteredIndex-PayRateTemplateDetail-PayRateTemplateId");

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.PayRateTemplateId)
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

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.ModifiedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.ModifiedDate)
                .HasColumnType("DATETIME2(3)");
        }
    }
}
