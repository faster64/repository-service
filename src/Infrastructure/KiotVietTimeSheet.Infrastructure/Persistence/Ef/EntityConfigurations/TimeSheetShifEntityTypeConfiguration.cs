using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class TimeSheetShiftEntityTypeConfiguration : IEntityTypeConfiguration<TimeSheetShift>
    {
        /// <summary>
        /// Special configurations define for TimeSheet entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TimeSheetShift> builder)
        {
            builder.ToTable("TimeSheetShift")
                .HasKey(t => t.Id);

            builder.ToTable("TimeSheetShift")
                .HasIndex(p => p.TimeSheetId)
                .HasName("NonClusteredIndex-TimeSheet-TimeSheetShift");

            builder.Property(t => t.ShiftIds)
                .IsRequired()
                .HasColumnType("NVARCHAR(1000)");

            builder.Property(t => t.TimeSheetId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.RepeatDaysOfWeek)
                .HasColumnType("NVARCHAR(500)");


            //ignore Property reference
            builder.Ignore(t => t.ShiftIdsToList);
            builder.Ignore(t => t.RepeatDaysOfWeekInList);

        }
    }
}
