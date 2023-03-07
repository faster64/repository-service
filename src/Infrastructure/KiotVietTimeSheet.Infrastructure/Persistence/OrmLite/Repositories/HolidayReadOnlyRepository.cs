using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using ServiceStack.Data;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class HolidayReadOnlyRepository : OrmLiteRepository<Holiday, long>, IHolidayReadOnlyRepository
    {
        public HolidayReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService)
            : base(db, authService)
        {
        }

        public virtual async Task<PagingDataSource<HolidayDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<HolidayDto>(query);
        }

        public async Task<List<Holiday>> GetByTimeAndTenantIdAsync(DateTime startDayYear, DateTime startEndYear, int tenantId)
        {
            var specificationHoliday =
                ((new FindHolidayByFromYearEqualSpec(startDayYear).And(new FindHolidayByFromLessThanSpec(startEndYear)))
                    .Or(new FindHolidayByToYearEqualSpec(startDayYear).And(new FindHolidayByToLessThanSpec(startEndYear))))
                .And(new FindHolidayByTenantIdSpec(tenantId));

            return await Db.SelectAsync(specificationHoliday.GetExpression());
        }
    }
}
