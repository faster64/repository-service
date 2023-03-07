using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class PayslipEntityTypeConfiguration : IEntityTypeConfiguration<Payslip>
    {
        public void Configure(EntityTypeBuilder<Payslip> builder)
        {
            builder
                .ToTable("Payslip")
                .HasKey(entity => entity.Id);

            builder
                .Property(entity => entity.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Payslip).Name}Seq");

            builder
                .ToTable("Payslip")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-Payslip-EmployeeId");

            builder
                .ToTable("Payslip")
                .HasIndex(p => p.PaysheetId)
                .HasName("NonClusteredIndex-Payslip-PaysheetId");

            builder
                .ToTable("Payslip")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Payslip-TenantId");

            builder.ToTable("Payslip")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Payslip-TenantId-Code-Uniq");

            builder
                .Property(entity => entity.Code)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder
                .Property(entity => entity.PayslipStatus)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder
                .Property(entity => entity.Note)
                .HasColumnType("NVARCHAR(2000)");

            builder
                .Property(entity => entity.PaysheetId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder
                .Property(entity => entity.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder
                .Property(entity => entity.Allowance)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.Deduction)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.CommissionSalary)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.OvertimeSalary)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.MainSalary)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.TotalPayment)
                .IsRequired()
                .HasDefaultValueSql("0")
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.Bonus)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.NetSalary)
                .IsRequired()
                .HasColumnType("MONEY");

            builder
                .Property(entity => entity.GrossSalary)
                .IsRequired()
                .HasColumnType("MONEY");

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

            builder
                .Property(entity => entity.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.IsDraft)
                .HasColumnType("BIT");

            builder.Property(t => t.PayslipCreatedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.PayslipCreatedBy)
                .HasColumnType("BIGINT");

            builder
                .HasMany(t => t.PayslipDetails)
                .WithOne();
        }
    }
}
