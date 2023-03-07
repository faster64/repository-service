using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class CommissionBranchEntityTypeConfiguration : IEntityTypeConfiguration<CommissionBranch>
    {
        public void Configure(EntityTypeBuilder<CommissionBranch> builder)
        {
            builder.ToTable("CommissionBranch")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(CommissionBranch).Name}Seq");

            builder.ToTable("CommissionBranch")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-CommissionBranch-TenantId");

            builder.ToTable("CommissionBranch")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-CommissionBranch-BranchId");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.CommissionId)
                .IsRequired()
                .HasColumnType("BIGINT");
        }
    }
}
