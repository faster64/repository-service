using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class NationalHolidayEntityTypeConfiguration : IEntityTypeConfiguration<NationalHoliday>
    {
        /// <summary>
        /// Special configurations define for NationalHoliday entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<NationalHoliday> builder)
        {
            builder.ToTable("NationalHoliday")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnType("INT")
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(250)");

            builder.Property(t => t.StartDay)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.EndDay)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.StartMonth)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.EndMonth)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.IsLunarCalendar)
                .IsRequired()
                .HasColumnType("BIT");
        }
    }
}
