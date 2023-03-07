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
    public class PayslipClockingReadOnlyRepository : OrmLiteRepository<PayslipClocking, long>, IPayslipClockingReadOnlyRepository
    {
        public PayslipClockingReadOnlyRepository(IDbConnectionFactory db, IAuthService authService) : base(db, authService) { }

        public async Task<PagingDataSource<PayslipClocking>> GetPayslipsClockingByPayslipIdAsync(
            PayslipClockingByPayslipIdFilter filter)
        {
            ISpecification<PayslipClocking> spec = new DefaultTrueSpec<PayslipClocking>();

            if (filter.PayslipId.HasValue)
                spec = spec.And(new FindPayslipClockingByPayslipIdSpec(filter.PayslipId.Value));

            var query = BuildPagingFilterSqlExpression(filter)
                .And<PayslipClocking>(spec.GetExpression());

            var dataSource = await LoadSelectDataSourceAsync<PayslipClocking>(query);
            return dataSource;
        }
    }
}
