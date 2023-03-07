using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class EmployeeBranchEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeBranch>
    {
        public void Configure(EntityTypeBuilder<EmployeeBranch> builder)
        {
            builder.ToTable("EmployeeBranch")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(EmployeeBranch).Name}Seq");

            builder.ToTable("EmployeeBranch")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-EmployeeBranch-TenantId");

            builder.ToTable("EmployeeBranch")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-EmployeeBranch-BranchId");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");
        }
    }
}
