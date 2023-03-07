using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IGpsInfoReadOnlyRepository : IBaseReadOnlyRepository<GpsInfo, long>
    {
        Task<PagingDataSource<GpsInfo>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null);
        Task<GpsInfo> GetForClockingGps(TenantModel tenant, string qrKey);
        Task<List<GpsInfo>> GetListForClockingGps(TenantModel tenant);
        Task<GpsInfo> GetGpsInforByBranchId(int tenantId, int branchId);
    }
}
