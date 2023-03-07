using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    class FingerPrintEntityConfiguration : IEntityTypeConfiguration<FingerPrint>
    {
        public void Configure(EntityTypeBuilder<FingerPrint> builder)
        {
            builder.ToTable("FingerPrint")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(FingerPrint).Name}Seq");

            builder.ToTable("FingerPrint")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-FingerPrint-TenantId");

            builder.ToTable("FingerPrint")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-FingerPrint-BranchId");

            builder.ToTable("FingerPrint")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-FingerPrint-EmployeeId");

            builder.Property(t => t.FingerName)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.FingerCode)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.FingerNo)
                .IsRequired()
                .HasDefaultValue(0)
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
        }
    }
}
