using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ClockingEntityTypeConfiguration : IEntityTypeConfiguration<Clocking>
    {
        /// <summary>
        /// Special configurations define for Clocking entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Clocking> builder)
        {
            builder.ToTable("Clocking")
                .HasKey(t => t.Id);

            builder.ToTable("Clocking")
                .HasIndex(p => p.TimeSheetId)
                .HasName("NonClusteredIndex-Clocking-TimeSheetId");

            builder.ToTable("Clocking")
                .HasIndex(p => p.ShiftId)
                .HasName("NonClusteredIndex-Clocking-ShiftId");

            builder.ToTable("Clocking")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-Clocking-EmployeeId");

            builder.ToTable("Clocking")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Clocking-TenantId");

            builder.ToTable("Clocking")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-Clocking-BranchId");

            builder.ToTable("Clocking")
               .HasIndex(p => p.WorkById)
               .HasName("NonClusteredIndex-Clocking-WorkById");

            builder.ToTable("Clocking")
                .HasOne(x => x.WorkBy)
                .WithMany(c => c.Clockings)
                .HasForeignKey(c => c.WorkById);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Clocking).Name}Seq");

            builder.Property(t => t.TimeSheetId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.ShiftId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.WorkById)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.ClockingStatus)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.StartTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.EndTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(500)");

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

            builder.Property(t => t.IsDeleted)
               .HasColumnType("BIT");

            builder.Property(t => t.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.DeletedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.TimeIsLate)
                .IsRequired()
                .HasColumnType("INT")
                .HasDefaultValue(0);

            builder.Property(t => t.OverTimeAfterShiftWork)
                .IsRequired()
                .HasColumnType("INT")
                .HasDefaultValue(0);

            builder.Property(t => t.OverTimeBeforeShiftWork)
                .IsRequired()
                .HasColumnType("INT")
                .HasDefaultValue(0);

            builder.Property(t => t.TimeIsLeaveWorkEarly)
                .IsRequired()
                .HasColumnType("INT")
                .HasDefaultValue(0);

            builder.Property(t => t.AbsenceType)
                .HasColumnType("TINYINT")
                .HasDefaultValue(null);

            builder.Property(t => t.ClockingPaymentStatus)
                .IsRequired()
                .HasColumnType("TINYINT")
                .HasDefaultValue(0);

            builder.Property(t => t.CheckInDate)
                .HasColumnType("DATETIME2(3)")
                .HasDefaultValue(null);

            builder.Property(t => t.CheckOutDate)
                .HasColumnType("DATETIME2(3)")
                .HasDefaultValue(null);

            // ignore Property reference
            builder.Ignore(t => t.Employee);

        }

    }
}
