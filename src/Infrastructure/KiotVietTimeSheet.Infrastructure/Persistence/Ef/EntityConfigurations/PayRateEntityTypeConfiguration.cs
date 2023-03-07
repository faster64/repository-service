using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayRateEntityTypeConfiguration : IEntityTypeConfiguration<PayRate>
    {
        /// <inheritdoc />
        /// <summary>
        /// Special configurations define for PayRate entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<PayRate> builder)
        {
            builder.ToTable("PayRate")
               .HasKey(t => t.Id);

            builder.Property(t => t.Id)
              .ForSqlServerUseSequenceHiLo($"{typeof(PayRate).Name}Seq");

            builder.ToTable("PayRate")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-PayRate-TenantId");

            builder.ToTable("PayRate")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-PayRate-EmployeeId");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.PayRateTemplateId)
                .HasColumnType("BIGINT");

            builder.Property(t => t.SalaryPeriod)
                .IsRequired()
                .HasColumnType("TINYINT");

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

            builder
                .HasMany(t => t.PayRateDetails)
                .WithOne();
        }
    }
}
