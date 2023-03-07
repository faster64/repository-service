using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PenalizeEntityTypeConfiguration : IEntityTypeConfiguration<Penalize>
    {
        /// <summary>
        /// Special configurations define for Penalize entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Penalize> builder) 
        {
            builder.ToTable("Penalize")
                .HasKey(t => t.Id);

            builder.ToTable("Penalize")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Penalize-TenantId");

            builder.ToTable("Penalize")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Penalize-TenantId-Code-Uniq");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Penalize).Name}Seq");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Value)
                .IsRequired()
                .HasColumnType("DECIMAL(18, 2)");

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
