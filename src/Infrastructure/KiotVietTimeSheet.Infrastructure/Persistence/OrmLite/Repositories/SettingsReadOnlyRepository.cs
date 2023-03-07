using KiotVietTimeSheet.Application.Auth;
using ServiceStack.Data;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using System.Linq;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class SettingsReadOnlyRepository : OrmLiteRepository<Settings, long>, ISettingsReadOnlyRepository
    {
        public SettingsReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
           : base(db, authService)
        {
        }

        public async Task<bool> IsUseClockingGps(int tenantId)
        {
            var query = Db
                .From<Settings>()
                .Where(x =>
                    x.TenantId == tenantId
                    && x.Name == "UseClockingGps"
                    && x.Value == "true"
                );

            var result = (await Db.LoadSelectAsync(query)).FirstOrDefault() != null;

            return result;
        }
    }
}
