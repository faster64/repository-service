using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.DbMaster
{
    public class DbMasterContext : DbContext
    {
        public DbMasterContext(DbContextOptions<DbMasterContext> options)
            : base(options)
        {

        }

        public DbSet<KvGroup> KvGroup { get; set; }

        public DbSet<KvRetailer> KvRetailer { get; set; }
    }
}
