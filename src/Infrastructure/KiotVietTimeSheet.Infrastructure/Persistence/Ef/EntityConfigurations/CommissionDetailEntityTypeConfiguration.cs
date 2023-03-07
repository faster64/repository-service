using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class CommissionDetailEntityTypeConfiguration : IEntityTypeConfiguration<CommissionDetail>
    {
        public void Configure(EntityTypeBuilder<CommissionDetail> builder)
        {
            builder.ToTable("CommissionDetail")
                .HasKey(t => t.Id);

            builder.ToTable("CommissionDetail")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-CommissionDetail-TenantId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(CommissionDetail).Name}Seq");

            builder.Property(t => t.CommissionId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.ObjectId)
                .HasColumnType("BIGINT");

            builder.Property(t => t.Value)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.ValueRatio)
                .HasColumnType("decimal(18,2)");

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
