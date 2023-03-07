using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class GpsInfoEntityTypeConfiguration : IEntityTypeConfiguration<GpsInfo>
    {
        public void Configure(EntityTypeBuilder<GpsInfo> builder)
        {
            builder.ToTable("GpsInfo")
                .HasKey(t => t.Id);

            builder.ToTable("GpsInfo")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-GpsInfo-TenantId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(GpsInfo).Name}Seq");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.BranchId)
                .IsRequired()
                .HasColumnType("INT");

            builder.Property(t => t.Coordinate)
                .HasColumnType("NVARCHAR(2000)");

            builder.Property(t => t.Address)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.LocationName)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.WardName)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.Province)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.District)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.Status)
                .IsRequired()
                .HasColumnType("TINYINT");

            builder.Property(t => t.QrKey)
                .IsRequired()
                .HasColumnType("NVARCHAR(100)");

            builder.Property(t => t.RadiusLimit)
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
