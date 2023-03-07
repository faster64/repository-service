using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class ShiftEntityTypeConfiguration : IEntityTypeConfiguration<Shift>
    {
        /// <summary>
        /// Special configurations define for Shift entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("Shift")
                .HasKey(shift => shift.Id);

            builder.ToTable("Shift")
                .HasIndex(shift => shift.TenantId)
                .HasName("NonClusteredIndex-Shift-TenantId");

            builder.ToTable("Shift")
                .HasIndex(shift => shift.BranchId)
                .HasName("NonClusteredIndex-Shift-BranchId");

            builder.Property(shift => shift.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Shift).Name}Seq");

            builder.Property(shift => shift.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(250)");

            builder.Property(shift => shift.To)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.From)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.IsActive)
                .IsRequired()
                .HasColumnType("BIT");

            builder.Property(shift => shift.IsDeleted)
                .HasColumnType("BIT");

            builder.Property(shift => shift.BranchId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(shift => shift.CheckInBefore)
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.CheckOutAfter)
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(shift => shift.CreatedBy)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("DATETIME2(3)");

            builder.Property(shift => shift.ModifiedBy)
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.ModifiedDate)
                .HasColumnType("DATETIME2(3)");

            builder.Property(shift => shift.DeletedBy)
                .HasColumnType("BIGINT");

            builder.Property(shift => shift.DeletedDate)
                .HasColumnType("DATETIME2(3)");
        }
    }
}
