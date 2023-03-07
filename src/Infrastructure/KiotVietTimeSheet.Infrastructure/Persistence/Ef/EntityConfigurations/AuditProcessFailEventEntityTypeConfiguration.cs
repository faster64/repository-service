using KiotVietTimeSheet.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.EntityConfigurations
{
    public class AuditProcessFailEventEntityTypeConfiguration : IEntityTypeConfiguration<AuditProcessFailEvent>
    {
        /// <inheritdoc />
        /// <summary>
        /// Special configurations define for AuditProcessFailEvent entity
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<AuditProcessFailEvent> builder)
        {
            builder.ToTable("AuditProcessFailEvent")
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .UseSqlServerIdentityColumn();

            builder.ToTable("AuditProcessFailEvent")
                .HasIndex(p => p.EventId)
                .HasName("NonClusteredIndex-AuditProcessFailEvent-EventId");

            builder.ToTable("AuditProcessFailEvent")
                .HasIndex(p => p.State)
                .HasName("NonClusteredIndex-AuditProcessFailEvent-State");

            builder.Property(t => t.RetryTimes)
                .IsRequired();

            builder.Property(t => t.EventType)
                .IsRequired()
                .HasColumnType("NVARCHAR(500)");

            builder.Property(t => t.ErrorMessage)
                .IsRequired()
                .HasColumnType("NTEXT");

            builder.Property(t => t.EventData)
               .IsRequired()
               .HasColumnType("NTEXT");

            builder.Property(t => t.State)
                .IsRequired();

            builder.Property(t => t.CreatedTime)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("DATETIME2(3)");

            builder.Property(t => t.ProcessedTime)
                .HasColumnType("DATETIME2(3)");
        }
    }
}
