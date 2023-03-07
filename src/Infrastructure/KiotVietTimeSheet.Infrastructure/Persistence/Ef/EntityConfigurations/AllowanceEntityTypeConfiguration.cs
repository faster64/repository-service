using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class AllowanceEntityTypeConfiguration : IEntityTypeConfiguration<Allowance>
    {
        /// <summary>
        /// Special configurations define for Allowance entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Allowance> builder)
        {
            builder.ToTable("Allowance")
                .HasKey(t => t.Id);

            builder.ToTable("Allowance")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Allowance-TenantId");

            builder.ToTable("Allowance")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Allowance-TenantId-Code-Uniq");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Allowance).Name}Seq");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(100)");

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
