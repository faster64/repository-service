using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class TenantNationalHolidayEntityTypeConfiguration : IEntityTypeConfiguration<TenantNationalHoliday>
    {
        /// <summary>
        /// Special configurations define for TenantNationalHoliday entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TenantNationalHoliday> builder)
        {
            builder.ToTable("TenantNationalHoliday")
                .HasKey(t => t.Id);

            builder.ToTable("TenantNationalHoliday")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-TenantNationalHoliday-TenantId");

            builder.Property(t => t.Id)
                .HasColumnType("BIGINT")
                .UseSqlServerIdentityColumn();

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.LastCreatedYear)
                .IsRequired()
                .HasColumnType("INT");

        }
    }
}
