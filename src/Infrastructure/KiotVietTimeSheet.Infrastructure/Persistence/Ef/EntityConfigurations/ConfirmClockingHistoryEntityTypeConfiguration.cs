using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ConfirmClockingHistoryEntityTypeConfiguration : IEntityTypeConfiguration<ConfirmClockingHistory>
    {
        public void Configure(EntityTypeBuilder<ConfirmClockingHistory> builder)
        {
            builder.ToTable("ConfirmClockingHistory")
                .HasKey(t => t.Id);

            builder.ToTable("ConfirmClockingHistory")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-ConfirmClockingHistory-TenantId");

            builder.ToTable("ConfirmClockingHistory")
                .HasIndex(p => p.ConfirmClockingId)
                .HasName("NonClusteredIndex-ConfirmClockingHistory-ConfirmClockingId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(ConfirmClockingHistory).Name}Seq");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.ConfirmClockingId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.ConfirmBy)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.ConfirmDate)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.StatusOld)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.StatusNew)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(2000)");

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
        }
    }
}
