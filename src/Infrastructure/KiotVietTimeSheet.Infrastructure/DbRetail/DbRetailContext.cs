using KiotVietTimeSheet.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.DbRetail
{
    public class DbRetailContext : DbContext
    {
        public DbRetailContext(DbContextOptions<DbRetailContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportExportFile>(entity =>
            {
                entity.Property(e => e.Revision)
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();
            });
        }

        public DbSet<ImportExportFile> ImportExportFile { get; set; }
    }
}
