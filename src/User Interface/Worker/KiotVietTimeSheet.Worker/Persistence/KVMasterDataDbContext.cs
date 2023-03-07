using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.AuditTrailWorker.Persistence
{
    public class KvMasterDataDbContext : DbContext
    {
        public KvMasterDataDbContext(DbContextOptions<KvMasterDataDbContext> options) : base(options) { }
        public DbSet<KvGroup> KvGroup { get; set; }
        public DbSet<KvRetailer> KvRetailer { get; set; }
    }
}
