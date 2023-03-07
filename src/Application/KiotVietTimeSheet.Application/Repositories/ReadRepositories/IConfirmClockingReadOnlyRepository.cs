using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IConfirmClockingReadOnlyRepository : IBaseReadOnlyRepository<ConfirmClocking, long>
    {
        Task<PagingDataSource<ConfirmClocking>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null);
        Task<List<ConfirmClockingDto>> GetConfirmClockingForClockingGps(int tenantId, int branchId, long employeeId, DateTime start, DateTime end, bool includeSoftDelete = false);
        Task<List<ConfirmClockingDto>> GetConfirmClockingsByBranchId(int branchId, bool includeSoftDelete = false);
    }
}
