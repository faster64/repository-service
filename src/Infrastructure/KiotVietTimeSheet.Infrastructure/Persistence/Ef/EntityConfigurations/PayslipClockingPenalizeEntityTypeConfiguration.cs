using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayslipClockingPenalizeEntityTypeConfiguration : IEntityTypeConfiguration<PayslipClockingPenalize>
    {
        /// <summary>
        /// Special configurations define for PayslipClockingPenalize entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<PayslipClockingPenalize> builder)
        {
            builder.ToTable("PayslipClockingPenalize")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(PayslipClockingPenalize).Name}Seq");

            builder.ToTable("PayslipClockingPenalize")
                .HasIndex(p => p.PayslipId)
                .HasName("NonClusteredIndex-PayslipClockingPenalize-PayslipId");

            builder.ToTable("PayslipClockingPenalize")
                .HasIndex(p => p.PenalizeId)
                .HasName("NonClusteredIndex-PayslipClockingPenalize-PenalizeId");

            builder.Property(t => t.ClockingId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.PayslipId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.PenalizeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.PenalizeName)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.TimesCount)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.Value)
                .IsRequired()
                .HasColumnType("DECIMAL(18, 2)");

            builder.Property(t => t.ClockingPenalizeCreated)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.MoneyType)
                .IsRequired()
                .HasDefaultValue((byte)MoneyTypes.Money)
                .HasColumnType("INT");

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasColumnType("BIGINT");

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

            builder.Property(t => t.ShiftId)
                .HasDefaultValue()
                .HasColumnType("BIGINT");
        }
    }
}
