using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayslipClockingEntityTypeConfiguration : IEntityTypeConfiguration<PayslipClocking>
    {
        public void Configure(EntityTypeBuilder<PayslipClocking> builder)
        {
            builder.ToTable("PayslipClocking")
                .HasKey(t => t.Id);

            builder.ToTable("PayslipClocking")
                .HasIndex(p => p.PayslipId)
                .HasName("NonClusteredIndex-PayslipClocking-PayslipId");

            builder.ToTable("PayslipClocking")
                .HasIndex(p => p.ClockingId)
                .HasName("NonClusteredIndex-PayslipClocking-ClockingId");

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.PayslipId)
                .IsRequired();

            builder.Property(t => t.ClockingId)
                .IsRequired();

            builder.Property(t => t.TimeIsLate)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeBeforeShiftWork)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.TimeIsLeaveWorkEarly)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeAfterShiftWork)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.AbsenceType)
                .HasColumnType("TINYINT");


            builder.Property(t => t.ClockingStatus)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.StartTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.EndTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.CheckInDate)
                .HasColumnType("DATETIME2(3)")
                .HasDefaultValue(null);

            builder.Property(t => t.CheckOutDate)
                .HasColumnType("DATETIME2(3)")
                .HasDefaultValue(null);

            builder.Property(t => t.ShiftId)
                .IsRequired()
                .HasColumnType("BIGINT");
        }
    }
}
