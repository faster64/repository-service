using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class FingerMachineEntityConfiguration : IEntityTypeConfiguration<FingerMachine>
    {
        public void Configure(EntityTypeBuilder<FingerMachine> builder)
        {
            builder.ToTable("FingerMachine")
                .HasKey(t => t.Id);

            builder.ToTable("FingerMachine")
                .HasIndex(t => new { t.TenantId, t.MachineId })
                .IsUnique();

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.ToTable("FingerMachine")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-FingerMachine-TenantId");

            builder.ToTable("FingerMachine")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-FingerMachine-BranchId");

            builder.Property(t => t.MachineName)
                .IsRequired()
                .HasColumnType("NVARCHAR(50)");

            builder.Property(t => t.MachineId)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasDefaultValue(1)
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

            builder.Property(t => t.Vendor)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.IpAddress)
                .HasColumnType("VARCHAR(100)");

            builder.Property(t => t.Commpass)
                .HasColumnType("INT");

            builder.Property(t => t.Port)
                .HasColumnType("INT");
        }
    }
}
