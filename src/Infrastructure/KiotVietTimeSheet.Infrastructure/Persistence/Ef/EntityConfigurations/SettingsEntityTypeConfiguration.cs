using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class SettingsEntityTypeConfiguration : IEntityTypeConfiguration<Settings>
    {
        /// <summary>
        /// Special configurations define for Settings entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Settings> builder)
        {
            builder.ToTable("Settings")
                .HasKey(t => t.Id);

            builder.ToTable("Settings")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-BranchSetting-TenantId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Settings).Name}Seq");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");
            builder.Property(t => t.Name)
                .HasColumnType("NVARCHAR(250)");
            builder.Property(t => t.Value)
                .HasColumnType("NVARCHAR(500)");
            builder.Property(t => t.IsActive)
                .HasColumnType("BIT");
            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("DATETIME2(3)");
        }
    }
}
