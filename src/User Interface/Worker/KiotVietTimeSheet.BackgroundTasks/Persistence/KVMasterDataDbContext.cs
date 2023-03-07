using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.BackgroundTasks.Persistence
{
    public class KvMasterDataDbContext : DbContext
    {
        public KvMasterDataDbContext(DbContextOptions<KvMasterDataDbContext> options) : base(options) { }
    }
}
