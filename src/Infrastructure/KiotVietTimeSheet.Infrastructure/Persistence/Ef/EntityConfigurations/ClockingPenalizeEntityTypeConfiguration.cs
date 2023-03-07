using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ClockingPenalizeEntityTypeConfiguration : IEntityTypeConfiguration<ClockingPenalize>
    {
        /// <summary>
        /// Special configurations define for ClockingPenalize entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<ClockingPenalize> builder)
        {
            builder.ToTable("ClockingPenalize")
                .HasKey(t => t.Id);

            builder.ToTable("ClockingPenalize")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-ClockingPenalize-TenantId");
                
            builder.ToTable("ClockingPenalize")
                .HasIndex(p => p.PenalizeId)
                .HasName("NonClusteredIndex-ClockingPenalize-PenalizeId");
            
            builder.ToTable("ClockingPenalize")
                .HasIndex(p => p.ClockingId)
                .HasName("NonClusteredIndex-ClockingPenalize-ClockingId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(ClockingPenalize).Name}Seq");

            builder.Property(t => t.ClockingId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.PenalizeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.TimesCount)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.Value)
                .IsRequired()
                .HasColumnType("DECIMAL(18, 2)");

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.ModifiedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.ModifiedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.IsDeleted)
                .HasColumnType("BIT");

            builder.Property(t => t.DeletedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.MoneyType)
                .IsRequired()
                .HasDefaultValue((byte)MoneyTypes.Money)
                .HasColumnType("INT");

            builder.Property(t => t.ClockingPaymentStatus)
                .IsRequired()
                .HasColumnType("TINYINT")
                .HasDefaultValue(0);
        }
    }
}
