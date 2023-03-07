using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class TimeSheetEntityTypeConfiguration : IEntityTypeConfiguration<TimeSheet>
    {
        /// <summary>
        /// Special configurations define for TimeSheet entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TimeSheet> builder)
        {
            builder.ToTable("TimeSheet")
                .HasKey(t => t.Id);

            builder.ToTable("TimeSheet")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-TimeSheet-TenantId");

            builder.ToTable("TimeSheet")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-TimeSheet-BranchId");

            builder.ToTable("TimeSheet")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-TimeSheet-EmployeeId");

            builder.ToTable("TimeSheet")
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(c => c.EmployeeId);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(TimeSheet).Name}Seq");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.StartDate)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.EndDate)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.IsRepeat)
                .HasColumnType("BIT");

            builder.Property(t => t.RepeatType)
                .HasColumnType("TINYINT");

            builder.Property(t => t.RepeatEachDay)
                .HasColumnType("TINYINT");


            builder.Property(t => t.IsDeleted)
                .HasColumnType("BIT");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

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

            builder.Property(t => t.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.DeletedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.TimeSheetStatus)
                .IsRequired()
                .HasDefaultValueSql("1")
                .HasColumnType("TINYINT");

            builder.Property(t => t.SaveOnHoliday)
                .HasColumnType("BIT");

            builder.Property(t => t.SaveOnDaysOffOfBranch)
                .HasColumnType("BIT");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(250)");
            builder
                .HasMany(t => t.TimeSheetShifts)
                .WithOne();
            //ignore Property reference
            builder.Ignore(t => t.Employee);
            builder.Ignore(t => t.TemporaryId);
        }
    }
}
