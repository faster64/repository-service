using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack.Data;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class PayslipClockingPenalizeReadOnlyRepository : OrmLiteRepository<PayslipClockingPenalize, long>, IPayslipClockingPenalizeReadOnlyRepository
    {
        public PayslipClockingPenalizeReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }

        public async Task<PagingDataSource<PayslipClockingPenalize>> GetPayslipsClockingPenalizeByPayslipIdAsync(
            PayslipClockingPenalizeByPayslipIdFilter filter)
        {
            ISpecification<PayslipClockingPenalize> spec = new DefaultTrueSpec<PayslipClockingPenalize>();

            if (filter.PayslipId.HasValue)
                spec = spec.And(new FindPayslipClockingPenalizeByPayslipIdSpec(filter.PayslipId.Value));

            var query = BuildPagingFilterSqlExpression(filter)
                .And<PayslipClockingPenalize>(spec.GetExpression());

            var dataSource = await LoadSelectDataSourceAsync<PayslipClockingPenalize>(query);
            return dataSource;
        }
    }
}
