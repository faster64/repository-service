using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class GpsInfoReadOnlyRepository : OrmLiteRepository<GpsInfo, long>, IGpsInfoReadOnlyRepository
    {
        public GpsInfoReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {

        }

        public virtual async Task<GpsInfo> GetForClockingGps(TenantModel tenant, string qrKey)
        {
            var query = Db.From<GpsInfo>().Where(x =>
                x.TenantId == tenant.Id
                && !x.IsDeleted
                && x.Status == (byte)GpsInfoStatus.Active
                && x.QrKey == qrKey);

            var result = (await Db.SelectAsync(query)).FirstOrDefault();

            return result;
        }

        public virtual async Task<List<GpsInfo>> GetListForClockingGps(TenantModel tenant)
        {
            var query = Db.From<GpsInfo>().Where(x =>
                x.TenantId == tenant.Id
                && x.Status == (byte)GpsInfoStatus.Active);

            var result = await Db.LoadSelectAsync(query);

            return result;
        }

        public async Task<PagingDataSource<GpsInfo>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null)
        {
            var gpsInfoDataSource = await LoadSelectDataSourceAsync<GpsInfo>(query, include, includeSoftDelete);
            return gpsInfoDataSource;
        }

        public async Task<GpsInfo> GetGpsInforByBranchId(int tenantId, int branchId)
        {
            var query = Db.From<GpsInfo>().Where(x =>
                x.TenantId == tenantId
                && x.BranchId == branchId
                && !x.IsDeleted
                && x.Status == (byte)GpsInfoStatus.Active);

            var result = await Db.LoadSelectAsync(query);
            return result.FirstOrDefault();
        }
    }
}
