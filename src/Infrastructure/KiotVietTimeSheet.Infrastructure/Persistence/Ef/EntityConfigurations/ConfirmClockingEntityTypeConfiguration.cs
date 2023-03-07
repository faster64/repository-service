using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ConfirmClockingEntityTypeConfiguration : IEntityTypeConfiguration<ConfirmClocking>
    {
        public void Configure(EntityTypeBuilder<ConfirmClocking> builder)
        {
            builder.ToTable("ConfirmClocking")
                .HasKey(t => t.Id);

            builder.ToTable("ConfirmClocking")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-ConfirmClocking-TenantId");

            builder.ToTable("ConfirmClocking")
                .HasIndex(p => p.GpsInfoId)
                .HasName("NonClusteredIndex-ConfirmClocking-GpsInfoId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(ConfirmClocking).Name}Seq");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.GpsInfoId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.CheckTime)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.Type)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(2000)");

            builder.Property(t => t.IdentityKeyClocking)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.Extra)
                .HasColumnType("NVARCHAR(4000)");

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
