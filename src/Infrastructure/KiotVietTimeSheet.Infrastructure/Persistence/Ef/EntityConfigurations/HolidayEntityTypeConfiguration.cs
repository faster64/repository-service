using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class HolidayEntityTypeConfiguration : IEntityTypeConfiguration<Holiday>
    {
        /// <summary>
        /// Special configurations define for Holiday entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable("Holiday")
                .HasKey(t => t.Id);

            builder.ToTable("Holiday")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-Holiday-TenantId");

            builder.Ignore(t => t.Days);

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(Holiday).Name}Seq");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("NVARCHAR(255)");

            builder.Property(t => t.To)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.From)
                .IsRequired()
                .HasColumnType("DATETIME2(3)");

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
