using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class ConfirmClockingReadOnlyRepository : OrmLiteRepository<ConfirmClocking, long>, IConfirmClockingReadOnlyRepository
    {
        private readonly IAuthService _authService;
        public ConfirmClockingReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {
            _authService = authService;
        }

        public async Task<PagingDataSource<ConfirmClocking>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false, string[] include = null)
        {
            var confirmClocking = await LoadSelectDataSourceAsync<ConfirmClocking>(query, include, includeSoftDelete);
            return confirmClocking;
        }

        public async Task<List<ConfirmClockingDto>> GetConfirmClockingForClockingGps(int tenantId, int branchId, long employeeId, DateTime start, DateTime end, bool includeSoftDelete = false)
        {
            var query = Db.From<ConfirmClocking>().Join<ConfirmClocking, GpsInfo>((cc, gi) => cc.GpsInfoId == gi.Id);
            query.Where<ConfirmClocking>(c => c.TenantId == tenantId)
            .Where<ConfirmClocking>(c => c.IsDeleted == includeSoftDelete)
            .Where<ConfirmClocking>(c => c.EmployeeId == employeeId)
            .Where<ConfirmClocking>(c => c.CheckTime >= start)
            .Where<ConfirmClocking>(c => c.CheckTime <= end)
            .Where<ConfirmClocking>(c => c.Status == (byte) ConfirmClockingStatus.Waiting)
            .Where<GpsInfo>(g => g.BranchId == branchId);

            var result = await Db.SelectAsync<ConfirmClockingDto>(query);
            return result;
        }

        public async Task<List<ConfirmClockingDto>> GetConfirmClockingsByBranchId(int branchId, bool includeSoftDelete = false)
        {
            var query = Db.From<ConfirmClocking>().Join<ConfirmClocking, GpsInfo>((cc, gi) => cc.GpsInfoId == gi.Id);
            query.Where<ConfirmClocking>(c => c.TenantId == _authService.Context.TenantId)
            .Where<ConfirmClocking>(c => c.IsDeleted == includeSoftDelete)
            .Where<ConfirmClocking>(c => c.Status == (byte)ConfirmClockingStatus.Waiting)
            .Where<GpsInfo>(g => g.BranchId == branchId);

            var result = await Db.SelectAsync<ConfirmClockingDto>(query);
            return result;
        }
    }
}
