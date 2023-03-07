using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
    {
        /// <summary>
        /// Special configurations define for JobTitle entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department")
                .HasKey(t => t.Id);

            builder.ToTable("Department")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Department-TenantId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Department).Name}Seq");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(50)");

            builder.Property(t => t.Description)
                .HasColumnType("NVARCHAR(500)");

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasColumnType("BIT");

            builder.Property(t => t.IsDeleted)
                .HasColumnType("BIT");

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

            builder.Property(t => t.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(t => t.DeletedDate)
                .HasColumnType("DATETIME2(3)");
        }
    }
}
