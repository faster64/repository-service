using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class BranchSettingEntityTypeConfiguration : IEntityTypeConfiguration<BranchSetting>
    {
        /// <summary>
        /// Special configurations define for BranchSetting entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<BranchSetting> builder)
        {
            builder.ToTable("BranchSetting")
                .HasKey(t => t.Id);

            builder.ToTable("BranchSetting")
                .HasIndex(p => new { p.TenantId, p.BranchId })
                .HasName("NonClusteredIndex-BranchSetting-TenantId-And-BranchId")
                .IsUnique();

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(BranchSetting).Name}Seq");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.WorkingDays)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");
        }
    }
}
