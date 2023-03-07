using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PaysheetEntityTypeConfiguration : IEntityTypeConfiguration<Paysheet>
    {
        public void Configure(EntityTypeBuilder<Paysheet> builder)
        {
            builder.ToTable("Paysheet")
               .HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Paysheet).Name}Seq");

            builder.ToTable("Paysheet")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Paysheet-TenantId");

            builder.ToTable("Paysheet")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-Paysheet-BranchId");

            builder.ToTable("Paysheet")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Paysheet-TenantId-Code-Uniq");

            builder.Property(t => t.Code)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(2000)");

            builder.Property(t => t.SalaryPeriod)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.PaysheetPeriodName)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.PaysheetStatus)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.CreatorBy)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.WorkingDayNumber)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.StartTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.EndTime)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.BranchId)
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

            builder.Property(t => t.Version)
                .IsRequired()
                .HasColumnType("BIGINT")
                .HasDefaultValue(0);

            builder.Property(t => t.IsDraft)
                .HasColumnType("BIT");

            builder.Property(t => t.PaysheetCreatedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.CreatorBy)
                .HasColumnType("BIGINT");

            builder
                .HasMany(t => t.Payslips)
                .WithOne();

            builder.Property(t => t.ErrorStatus)
                .HasColumnType("INT");

            builder.Ignore(c => c.EmployeeTotal);
            builder.Ignore(c => c.TotalNetSalary);
            builder.Ignore(c => c.TotalNeedPay);
            builder.Ignore(c => c.TotalPayment);
        }
    }
}
