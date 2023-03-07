using KiotVietTimeSheet.Infrastructure.EventBus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore
{
    public class EfIntegrationEventLogContext : DbContext
    {
        public EfIntegrationEventLogContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationEventLogEntry>(IntegrationEventLogEntityConfiguration);
        }

        private static void IntegrationEventLogEntityConfiguration(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable("IntegrationEventLogEntry")
                .HasKey(e => e.EventId);

            builder.ToTable("IntegrationEventLogEntry")
                .HasIndex(p => p.State)
                .HasName("NonClusteredIndex-IntegrationEventLogEntry-State");

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Data)
                .IsRequired();

            builder.Property(e => e.TimeSent)
                .IsRequired();

            builder.Property(e => e.CreatedTime)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.EventType)
                .IsRequired();

            builder.Property(e => e.TransactionId)
                .IsRequired();
        }
    }
}
