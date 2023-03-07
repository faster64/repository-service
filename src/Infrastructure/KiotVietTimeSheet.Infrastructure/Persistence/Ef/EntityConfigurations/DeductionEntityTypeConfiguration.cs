using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class DeductionEntityTypeConfiguration : IEntityTypeConfiguration<Deduction>
    {
        /// <summary>
        /// Special configurations define for Deduction entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Deduction> builder)
        {
            builder.ToTable("Deduction")
                .HasKey(t => t.Id);

            builder.ToTable("Deduction")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Deduction-TenantId");

            builder.ToTable("Deduction")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Deduction-TenantId-Code-Uniq");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Deduction).Name}Seq");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.Value)
                .HasColumnType("decimal(18,2)");

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

            builder.Property(t => t.IsDeleted)
                .HasColumnType("BIT");

            builder.Property(t => t.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.DeletedDate)
                .HasColumnType("DATETIME2(3)");
        }
    }
}
