using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class EmployeeProfilePictureEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeProfilePicture>
    {
        /// <summary>
        /// Special configurations define for EmployeeProfilePicture entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<EmployeeProfilePicture> builder)
        {
            builder.ToTable("EmployeeProfilePicture")
                .HasKey(t => t.Id);

            builder.ToTable("EmployeeProfilePicture")
                .HasIndex(p => p.TenantId)
                .HasName("NonClusteredIndex-EmployeeProfilePicture-TenantId");

            builder.ToTable("EmployeeProfilePicture")
                .HasIndex(p => p.EmployeeId)
                .HasName("NonClusteredIndex-EmployeeProfilePicture-EmployeeId");

            builder.Property(t => t.Id)
                .ForSqlServerUseSequenceHiLo($"{typeof(EmployeeProfilePicture).Name}Seq");

            builder.Property(t => t.ImageUrl)
                .IsRequired()
                .HasColumnType("NVARCHAR(4000)");

            builder.Property(t => t.IsMainImage)
                .HasColumnType("BIT");

            builder.Property(t => t.EmployeeId)
                .IsRequired()
                .HasColumnType("BIGINT");

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasColumnType("INT");
        }
    }
}
