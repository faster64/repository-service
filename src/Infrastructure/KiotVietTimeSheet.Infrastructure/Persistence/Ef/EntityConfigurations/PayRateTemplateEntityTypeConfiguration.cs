using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayRateTemplateEntityTypeConfiguration : IEntityTypeConfiguration<PayRateTemplate>
    {
        public void Configure(EntityTypeBuilder<PayRateTemplate> builder)
        {
            builder.ToTable("PayRateTemplate")
                .HasKey(t => t.Id);

            builder.ToTable("PayRateTemplate")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-PayRateTemplate-BranchId");

            builder.ToTable("PayRateTemplate")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-PayRateTemplate-TenantId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(PayRateTemplate).Name}Seq");

            builder
                .HasMany(t => t.PayRateTemplateDetails)
                .WithOne();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(250)");

            builder.Property(t => t.SalaryPeriod)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.BranchId)
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

            builder.Property(t => t.Status)
                .HasColumnType("INT");
        }
    }
}
