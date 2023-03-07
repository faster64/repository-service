using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class CommissionDetailReadOnlyRepository : OrmLiteRepository<CommissionDetail, long>, ICommissionDetailReadOnlyRepository
    {
        public CommissionDetailReadOnlyRepository(IDbConnectionFactory dbFactory, IAuthService authService) : base(dbFactory, authService)
        {
        }

        public async Task<PagingDataSource<CommissionDetailDto>> GetCommissionDetailByProductId(ISqlExpression query)
        {
            var q = (SqlExpression<CommissionDetail>)query;

            q = q.Join<CommissionDetail, Commission>((cd, c) => cd.CommissionId == c.Id)
                .Where<CommissionDetail, Commission>((cd, c) => c.IsActive && !c.IsDeleted)
                .SelectDistinct();

            var result = await LoadSelectDataSourceAsync<CommissionDetailDto>(q);
            return result;
        }

        public async Task<PagingDataSource<CommissionDetailDto>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<CommissionDetailDto>(query, null, includeSoftDelete);

        public async Task<PagingDataSource<CommissionDetail>> GetListByQueryFilterAsync(CommissionDetailQueryFilter filter)
        {
            ISpecification<CommissionDetail> specification =
                new DefaultTrueSpec<CommissionDetail>().And(
                    new FindCommissionDetailByTenantIdSpec(AuthService.Context.TenantId));

            specification = specification.And(new FindCommissionDetailByCommissionIdsSpec(filter.CommissionIds));

            var query = Db.From<CommissionDetail>(Db.TableAlias(typeof(CommissionDetail).Name))
                .Where<CommissionDetail>(specification.GetExpression());

            var totalQuery = query.Clone()
                .GroupBy(s => s.Id)
                .Select("COUNT(*) OVER ()");

            var dataSource = new PagingDataSource<CommissionDetail>
            {
                Total = await Db.SingleAsync<long>(totalQuery),
                Data = (await Db.SelectAsync(query)).ConvertTo<List<CommissionDetail>>()
            };

            return dataSource;
        }
    }
}
