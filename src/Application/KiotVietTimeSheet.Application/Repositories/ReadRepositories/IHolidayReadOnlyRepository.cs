using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IHolidayReadOnlyRepository : IBaseReadOnlyRepository<Holiday, long>
    {
        Task<PagingDataSource<HolidayDto>> FiltersAsync(ISqlExpression query);

        Task<List<Holiday>> GetByTimeAndTenantIdAsync(DateTime startDayYear, DateTime startEndYear, int tenantId);
    }
}
