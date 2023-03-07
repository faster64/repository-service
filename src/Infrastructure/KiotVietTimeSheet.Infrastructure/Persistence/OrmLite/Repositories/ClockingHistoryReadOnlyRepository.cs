using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class ClockingHistoryReadOnlyRepository : OrmLiteRepository<ClockingHistory, long>, IClockingHistoryReadOnlyRepository
    {
        public ClockingHistoryReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
           : base(db, authService)
        {
        }

        public async Task<PagingDataSource<ClockingHistoryDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<ClockingHistoryDto>(query);
        }
    }
}
