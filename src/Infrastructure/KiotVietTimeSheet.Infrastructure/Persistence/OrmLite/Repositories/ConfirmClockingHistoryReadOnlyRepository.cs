using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class ConfirmClockingHistoryReadOnlyRepository : OrmLiteRepository<ConfirmClockingHistory, long>, IConfirmClockingHistoryReadOnlyRepository
    {
        public ConfirmClockingHistoryReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {

        }

        public async Task<PagingDataSource<ConfirmClockingHistory>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null)
        {
            var confirmClockingHistory = await LoadSelectDataSourceAsync<ConfirmClockingHistory>(query, include, includeSoftDelete);
            return confirmClockingHistory;
        }
    }
}
