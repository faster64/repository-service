using System;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class TimeSheetReadOnlyRepository : OrmLiteRepository<TimeSheet, long>, ITimeSheetReadOnlyRepository
    {
        #region Properties
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        #endregion
        public TimeSheetReadOnlyRepository(
            IDbConnectionFactory db,
            IAuthService authService,
            IMapper mapper)
            : base(db, authService)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<PagingDataSource<TimeSheetDto>> FiltersAsync(ISqlExpression query)
        {
            SqlExpression<TimeSheet> q = (SqlExpression<TimeSheet>)query;
            var timeSheets = await Db.LoadSelectAsync(q);
            return new PagingDataSource<TimeSheetDto>
            {
                Total = (int)await Db.CountAsync(q),
                Data = _mapper.Map<List<TimeSheetDto>>(timeSheets)
            };
        }

        /// <inheritdoc />
        public async Task<List<TimeSheet>> GetTimeSheetByTimeSheets(List<TimeSheet> timeSheets)
        {
            if (timeSheets != null && timeSheets.Any())
            {
                var startDate = timeSheets.OrderBy(x => x.StartDate).First().StartDate;
                var endDate = timeSheets.OrderByDescending(x => x.EndDate).First().EndDate;
                var branchId = timeSheets.First().BranchId;
                var employeeIds = timeSheets.Select(x => x.EmployeeId).Distinct().ToList();
                var q = Db.From<TimeSheet>()
                    .Where(t => t.TenantId == AuthService.Context.TenantId
                                && t.BranchId == branchId
                                && t.StartDate >= startDate && t.EndDate <= endDate
                                && !t.IsDeleted && t.TimeSheetStatus != (byte)TimeSheetStatuses.Void
                                && employeeIds.Contains(t.EmployeeId));
                return await Db.LoadSelectAsync(q);
            }
            return new List<TimeSheet>();

        }

        /// <inheritdoc />
        public async Task<List<TimeSheet>> GetTimeSheetOverlayHoliday(DateTime from, DateTime to)
        {
            var specification = new LimitByTenantIdSpec<TimeSheet>(_authService.Context.TenantId)
                .And(new FilterSoftDeleteSpec<TimeSheet>())
                .And(new FindTimeSheetByStatusSpec((byte)TimeSheetStatuses.Created))
                .And(new FindTimeSheetByEndDateGreaterThanSpec(from.Date))
                .And(new FindTimeSheetByStartDateLessThanSpec(to.Date.AddDays(1)));

            var query = Db.From<TimeSheet>()
                .Join<TimeSheet, Employee>((t, e) => t.EmployeeId == e.Id && !e.IsDeleted && e.IsActive)
                .Where<TimeSheet>(specification.GetExpression())
                .Select();

            return await Db.LoadSelectAsync(query);
        }
    }
}
