using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ClockingHistoryEntityTypeConfiguration : IEntityTypeConfiguration<ClockingHistory>
    {
        /// <summary>
        /// Special configurations define for Clocking History entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<ClockingHistory> builder)
        {
            builder.ToTable("ClockingHistory")
                .HasKey(t => t.Id);

            builder.ToTable("ClockingHistory")
                .HasIndex(p => p.ClockingId)
                .HasName("NonClusteredIndex-ClockingHistory-ClockingId");

            builder.ToTable("ClockingHistory")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-ClockingHistory-TenantId");

            builder.ToTable("ClockingHistory")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-ClockingHistory-BranchId");

            builder.ToTable("ClockingHistory")
                .HasIndex(t => new { t.TenantId, t.ClockingId, t.AutoTimekeepingUid })
                .HasName("UniqueIndex-ClockingHistory-AutoTimekeepingUid")
                .IsUnique();

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(ClockingHistory).Name}Seq");

            builder.Property(t => t.ClockingId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.CheckedInDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.CheckedOutDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.CheckTime)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.TimeIsLate)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeAfterShiftWork)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeBeforeShiftWork)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.TimeIsLeaveWorkEarly)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

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

            builder.Property(t => t.TimeKeepingType)
                .IsRequired()
                .HasDefaultValueSql("1")
                .HasColumnType("TINYINT");

            builder.Property(t => t.ClockingStatus)
                .IsRequired()
                .HasDefaultValueSql("2")
                .HasColumnType("TINYINT");

            builder.Property(t => t.ClockingHistoryStatus)
               .IsRequired()
               .HasDefaultValueSql("1")
               .HasColumnType("TINYINT");

            builder.Property(t => t.TimeIsLateAdjustment)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeAfterShiftWorkAdjustment)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.OverTimeBeforeShiftWorkAdjustment)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.TimeIsLeaveWorkEarlyAdjustment)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.AbsenceType)
                .HasColumnType("TINYINT")
                .HasDefaultValue(null);

            builder.Property(t => t.ShiftId)
                .HasColumnType("BIGINT")
                .HasDefaultValue();

            builder.Property(t => t.ShiftFrom)
                .HasColumnType("BIGINT")
                .HasDefaultValue();

            builder.Property(t => t.ShiftTo)
                .HasColumnType("BIGINT")
                .HasDefaultValue();

            builder.Property(t => t.EmployeeId)
                .HasColumnType("BIGINT")
                .HasDefaultValue();

            builder.Property(t => t.AutoTimekeepingUid)
                .HasColumnType("NVARCHAR(255)")
                .HasDefaultValue();
        }
    }
}
