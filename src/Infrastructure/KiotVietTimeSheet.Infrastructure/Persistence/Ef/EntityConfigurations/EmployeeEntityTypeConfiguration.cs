using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
    {
        /// <summary>
        /// Special configurations define for Employee entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee")
                .HasKey(t => t.Id);

            builder.ToTable("Employee")
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasName("NonClusteredIndex-Employee-TenantId-Code-Uniq");

            builder.ToTable("Employee")
                .HasIndex(p => p.BranchId)
                .HasName("NonClusteredIndex-Employee-BranchId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Employee).Name}Seq");

            builder.Property(t => t.Code)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");
            
            builder.Property(t => t.NickName)
                .HasColumnType("NVARCHAR(255)");


            builder.Property(t => t.DOB)
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.Gender)
                .HasColumnType("BIT");

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasColumnType("BIT");

            builder.Property(t => t.IdentityNumber)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.MobilePhone)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Email)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Facebook)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Address)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.LocationName)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.WardName)
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.Note)
                .HasColumnType("NVARCHAR(500)");

            builder.Property(t => t.IdentityKeyClocking)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.AccountSecretKey)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.DepartmentId)
                .HasColumnType("BIGINT");

            builder.Property(t => t.JobTitleId)
                .HasColumnType("BIGINT");

            builder.Property(t => t.IsDeleted)
                .HasColumnType("BIT");

            builder.Property(t => t.UserId)
                .HasColumnType("BIGINT");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

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

            // ignore Property reference
            builder.Ignore(t => t.Clockings);
        }
    }
}
