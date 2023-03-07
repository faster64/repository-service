using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Utilities;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class ClockingReadOnlyRepository : OrmLiteRepository<Clocking, long>, IClockingReadOnlyRepository
    {
        private readonly IMapper _mapper;
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        private readonly IAuthService _authService;
        private readonly ILogger<ClockingReadOnlyRepository> _Logger;
        public ClockingReadOnlyRepository(IDbConnectionFactory db, IAuthService authService, IMapper mapper, IPenalizeReadOnlyRepository penalizeReadOnlyRepository, ILogger<ClockingReadOnlyRepository> logger)
           : base(db, authService)
        {
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;
            _mapper = mapper;
            _authService = authService;
            _Logger = logger;
        }

        public async Task<PagingDataSource<ClockingDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<ClockingDto>(query);
        }

        /// <inheritdoc />
        public async Task<PagingDataSource<ClockingDto>> GetListClockingForCalendar(ISqlExpression query, List<byte> clockingHistoryStates, List<long> departmentIds)
        {
            var queryExpressionClocking = (SqlExpression<Clocking>)query;

            if (clockingHistoryStates != null && clockingHistoryStates.Any())
            {
                queryExpressionClocking = queryExpressionClocking.Join<Clocking, ClockingHistory>((c, ch) =>
                    c.Id == ch.ClockingId && c.ClockingStatus != (byte)ClockingStatuses.Void);

                queryExpressionClocking = BuildSqlExpressionByClockingStatus(queryExpressionClocking, clockingHistoryStates).SelectDistinct();
            }
            else
            {
                queryExpressionClocking = queryExpressionClocking
                    .Join<Clocking, Employee>((c, em) => c.EmployeeId == em.Id)
                    .Where<Clocking, Employee>((c, em) => !em.IsDeleted || (em.IsDeleted && c.ClockingStatus != (byte)ClockingStatuses.Created))
                    .Where<Clocking>(c => c.ClockingStatus != (byte)ClockingStatuses.Void)
                    .SelectDistinct();
            }

            if (departmentIds != null && departmentIds.Any())
            {
                queryExpressionClocking = queryExpressionClocking.Where<Clocking, Employee>((c, em) => departmentIds.Contains((long)em.DepartmentId));
            }

            var result = await LoadSelectDataSourceAsync<ClockingDto>(queryExpressionClocking);
            if (result == null || !result.Data.Any()) return result;

            var timeSheetIds = result.Data.Select(x => x.TimeSheetId).Distinct().ToList();

            var queryTimeSheet = Db.From<TimeSheet>()
                .Where(t => t.TenantId == AuthService.Context.TenantId)
                .Where(t => timeSheetIds.Contains(t.Id));
            var timeSheets = await Db.LoadSelectAsync(queryTimeSheet);

            foreach (var clocking in result.Data)
            {
                clocking.TimeSheet = _mapper.Map<TimeSheetDto>(timeSheets.FirstOrDefault(x => x.Id == clocking.TimeSheetId));
                clocking.ClockingHistories = clocking.ClockingHistories?.OrderByDescending(x => x.Id).ToList() ?? new List<ClockingHistory>();
            }

            result.Data = result.Data.OrderBy(x => x.StartTime).ThenBy(x => x.EndTime).ToList();

            return result;
        }

        public async Task<PagingDataSource<ClockingDto>> GetClockingMultipleBranchForCalendars(List<int> branchIds, List<byte> clockingHistoryStates,
            List<long> departmentIds, List<long> shiftIds, List<long> employeeIds, DateTime startTime, DateTime endTime, List<byte> clockingStatusExtension)
        {
            var specification = new FindClockingByBranchIdsSpec(branchIds)
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(startTime))
                .And(new FindClockingByStartTimeLessThanOrEqualSpec(endTime));

            if (shiftIds != null && shiftIds.Any())
            {
                specification = specification.And(new FindClockingByShiftIdsSpec(shiftIds));
            }

            if (employeeIds != null && employeeIds.Any())
            {
                specification = specification.And(new FindClockingByEmployeeIdsSpec(employeeIds));
            }

            var result = await GetClockingDataSource(clockingStatusExtension, departmentIds, clockingHistoryStates, specification);

            var penalizes = await GetAllPenalizesByTenantId();

            if (result?.Data == null || !result.Data.Any()) return result;

            var queryTimeSheet = Db.From<TimeSheet>().Join<TimeSheet, Clocking>((ts, cl) => ts.Id == cl.TimeSheetId)
                .Where<Clocking>(specification.GetExpression())
                .Where<TimeSheet>(x => x.TenantId == AuthService.Context.TenantId);

            var timeSheets = await Db.LoadSelectAsync(queryTimeSheet);

            //Clocking has shift deleted but has not CheckedIn, CheckedOut or check WorkOff yet
            //We won't get it
            var listClockingStatus = new List<byte>()
            {
                (byte)ClockingStatuses.CheckedIn,
                (byte)ClockingStatuses.CheckedOut,
                (byte)ClockingStatuses.WorkOff
            };

            var clockingDtoFromSource =
                result.Data.Where(cl => listClockingStatus.Contains(cl.ClockingStatus))
                           .GroupBy(x => x.ShiftId)
                           .Select(x => x.Key)
                           .ToList();

            var queryShiftIsDelete =
                Db.From<Shift>()
                    .Where(x => !x.IsDeleted)
                    .Where(x => !x.IsActive)
                    .Where(x => branchIds.Contains(x.BranchId));

            var shifts = await Db.LoadSelectAsync(queryShiftIsDelete);

            var shiftIdShouldNotExists =
                shifts.Where(x => !clockingDtoFromSource.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(x => x.Key)
                    .ToList();

            if (shiftIdShouldNotExists.Any())
            {
                result.Data = result.Data.Where(x => !shiftIdShouldNotExists.Contains(x.ShiftId)).ToList();
            }

            foreach (var clocking in result.Data)
            {
                clocking.TimeSheet = _mapper.Map<TimeSheetDto>(timeSheets.FirstOrDefault(x => x.Id == clocking.TimeSheetId));
                clocking.ClockingHistories = clocking.ClockingHistories?.OrderByDescending(x => x.Id).ToList() ?? new List<ClockingHistory>();
                clocking.ClockingPenalizesDto = ConvertMapPenalizeDto(clocking.ClockingPenalizesDto, penalizes);
            }

            return result;
        }

        private List<ClockingPenalizeDto> ConvertMapPenalizeDto(List<ClockingPenalizeDto> clockingPenalizesDto, List<PenalizeDto> penalize)
        {
            if (clockingPenalizesDto == null || !clockingPenalizesDto.Any()) return new List<ClockingPenalizeDto>();

            var clockingPenalizes = clockingPenalizesDto.Where(x => !x.IsDeleted).ToList();

            foreach (var cpe in clockingPenalizes)
            {
                cpe.PenalizeDto = penalize.FirstOrDefault(x => x.Id == cpe.PenalizeId);
            }

            return clockingPenalizes;
        }

        private async Task<PagingDataSource<ClockingDto>> GetClockingDataSource(List<byte> clockingStatusExtension, List<long> departmentIds, List<byte> clockingHistoryStates, ISpecification<Clocking> specification)
        {
            var queryClocking = Db.From<Clocking>();

            var user = AuthService.Context.User;
            var employeeByUserQuery =
                Db.From<Employee>()
                    .Where(x => x.UserId == user.Id)
                    .Where(x => !x.IsDeleted)
                    .Where(x => x.IsActive);

            var employeeForUser = await Db.SingleAsync(employeeByUserQuery);
            //tach phan quyen voi gioi han giao dich
            var isLimitedByTrans = false;
            try
            {
                isLimitedByTrans = await _authService.HasPermissions(new[] { TimeSheetPermission.EmployeeLimit_Read });
            }
            catch (Exception epx)
            {
                isLimitedByTrans = !user.IsAdmin;
                _Logger.LogError(epx.Message, epx);
            }
            //lọc theo người dùng đăng nhập nếu không cho phép xem giao dịch của nhân viên khác
            if (!user.IsAdmin && isLimitedByTrans && employeeForUser == null)
            {
                return new PagingDataSource<ClockingDto>();
            }
            
            if (!user.IsAdmin && isLimitedByTrans && employeeForUser != null)
            {
                queryClocking = queryClocking.Where<Clocking>(ch => ch.EmployeeId == employeeForUser.Id);
            }

            queryClocking = queryClocking.Where<Clocking>(ch => ch.ClockingStatus != (byte)ClockingStatuses.Void);

            queryClocking = BuildSqlExpressionByClockingStatusExtension(queryClocking, clockingStatusExtension);

            if (departmentIds != null && departmentIds.Any())
            {
                queryClocking = queryClocking.Where<Clocking, Employee>((c, em) => departmentIds.Contains((long)em.DepartmentId));
            }

            //Nếu lựa chọn "Đi muộn, Về sớm, Làm thêm trước ca..."
            if (clockingHistoryStates != null && clockingHistoryStates.Any())
            {
                queryClocking = BuildSqlExpressionByClockingStatus(queryClocking, clockingHistoryStates);
            }
            queryClocking = queryClocking.Where<Clocking>(specification.GetExpression()).SelectDistinct();

            return await LoadSelectDataSourceAsync<ClockingDto>(queryClocking);

        }

        private async Task<List<PenalizeDto>> GetAllPenalizesByTenantId()
        {
            var result = await _penalizeReadOnlyRepository.GetAllAsync();
            return _mapper.Map<List<PenalizeDto>>(result);
        }

        private SqlExpression<Clocking> BuildSqlExpressionByClockingStatusExtension(SqlExpression<Clocking> queryClocking, List<byte> clockingStatusExtension)
        {
            if (clockingStatusExtension == null || !clockingStatusExtension.Any())
            {
                queryClocking = queryClocking
                    .Join<Clocking, Employee>((c, em) => c.EmployeeId == em.Id)
                    .Where<Clocking, Employee>((c, em) => !c.IsDeleted);
            }
            else
            {
                queryClocking = queryClocking.Join<Clocking, Employee>((c, em) => c.EmployeeId == em.Id)
                    .Where<Clocking, Employee>((c, em) =>
                    (
                        (clockingStatusExtension.Any(x => x == (byte)ClockingStatusesExtension.Created) && c.ClockingStatus == (byte)ClockingStatuses.Created && !em.IsDeleted)
                        || (clockingStatusExtension.Any(x => x == (byte)ClockingStatusesExtension.CheckInNoCheckOut) && c.ClockingStatus == (byte)ClockingStatuses.CheckedIn)
                        || (clockingStatusExtension.Any(x => x == (byte)ClockingStatusesExtension.CheckOutNoCheckIn) && c.ClockingStatus == (byte)ClockingStatuses.CheckedOut && c.CheckInDate == null)
                        || (clockingStatusExtension.Any(x => x == (byte)ClockingStatusesExtension.CheckInCheckOut) && c.ClockingStatus == (byte)ClockingStatuses.CheckedOut && c.CheckInDate != null)
                        || (clockingStatusExtension.Any(x => x == (byte)ClockingStatusesExtension.WorkOff) && c.ClockingStatus == (byte)ClockingStatuses.WorkOff)
                    ));
            }

            return queryClocking;
        }

        private SqlExpression<Clocking> BuildSqlExpressionByClockingStatus(SqlExpression<Clocking> queryClocking, List<byte> clockingHistoryStates)
        {
            queryClocking = queryClocking.Where<Clocking>(ch =>
                    (clockingHistoryStates.Any(x => x == (byte)ClockingHistoryState.BeLateToWork) && ch.TimeIsLate > 0) ||
                    (clockingHistoryStates.Any(x => x == (byte)ClockingHistoryState.OverTimeBeforeShiftWork) && ch.OverTimeBeforeShiftWork > 0) ||
                    (clockingHistoryStates.Any(x => x == (byte)ClockingHistoryState.LeaveWorkEarly) && ch.TimeIsLeaveWorkEarly > 0) ||
                    (clockingHistoryStates.Any(x => x == (byte)ClockingHistoryState.OverTimeAfterShiftWork) && ch.OverTimeAfterShiftWork > 0));

            return queryClocking;
        }

        /// <inheritdoc />
        public async Task<List<Clocking>> GetClockingOverlapHoliday(DateTime from, DateTime to)
        {
            var clockingSpecification = new LimitByTenantIdSpec<Clocking>(AuthService.Context.TenantId)
                .And(new FilterSoftDeleteSpec<Clocking>())
                .And(new FindClockingByStatusSpec((byte)ClockingStatuses.Created))
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(from.Date))
                .And(new FindClockingByStartTimeLessThanSpec(to.Date.AddDays(1)));

            // Danh sách clocking thuộc khoảng thời gian tạo holiday, chưa chấm công và thuộc timeSheet có thiết lập "Bỏ qua các ngày lễ tết",
            // không nằm trong ca làm việc ngừng hoạt động hoặc đã bị xóa
            var query = Db.From<Clocking>()
                .Join<Clocking, TimeSheet>((c, t) => c.TimeSheetId == t.Id && !t.SaveOnHoliday)
                .Join<Clocking, Shift>((c, s) => c.ShiftId == s.Id && !s.IsDeleted && s.IsActive)
                .Join<Clocking, Employee>((c, e) => c.EmployeeId == e.Id && !e.IsDeleted && e.IsActive)
                .Where<Clocking>(clockingSpecification.GetExpression())
                .Select();

            return await Db.LoadSelectAsync(query);
        }

        /// <inheritdoc />
        public async Task<List<Clocking>> GetClockingForPaySheet(DateTime from, DateTime to, List<long> employeeIds)
        {
            var getClockingSpecs = (
                    new FindClockingByStatusSpec((byte)ClockingStatuses.CheckedOut)
                        .Or(
                            new FindClockingByStatusSpec((byte)ClockingStatuses.WorkOff).And(
                                new FindClockingByAbsenceTypeSpec((byte)AbsenceTypes.AuthorisedAbsence))
                        )
                )
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(from))
                .And(new FindClockingByStartTimeLessThanSpec(to.Date.AddDays(1)))
                .And(new FindClockingByEmployeeIdsSpec(employeeIds))
                .And(new LimitByTenantIdSpec<Clocking>(AuthService.Context.TenantId))
                .And(new FilterSoftDeleteSpec<Clocking>());
            var query = Db.From<Clocking>()
                .Where<Clocking>(getClockingSpecs.GetExpression())
                .Select();
            return await Db.LoadSelectAsync(query);
        }

        public async Task<List<ClockingPenalizeDto>> GetClockingPenalizeForPaySheet(DateTime from, DateTime to, List<long> employeeIds)
        {
            var getClockingSpecs =
                new FindClockingByNotInStatusSpec((byte)ClockingStatuses.Void)
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(from))
                .And(new FindClockingByStartTimeLessThanSpec(to.Date.AddDays(1)))
                .And(new FindClockingByEmployeeIdsSpec(employeeIds))
                .And(new LimitByTenantIdSpec<Clocking>(AuthService.Context.TenantId))
                .And(new FilterSoftDeleteSpec<Clocking>());
            var query = Db.From<Clocking>()
                .Where<Clocking>(getClockingSpecs.GetExpression())
                .Select();

            var listClocking = await Db.LoadSelectAsync(query);

            var lstClockingPenalize = 
                    listClocking.Where(c => c.ClockingPenalizes != null)
                                .SelectMany(x => x.ClockingPenalizes)
                                .Where(x => x.ClockingPaymentStatus == (byte)ClockingPaymentStatuses.UnPaid)
                                .ToList();

            var listClockingPenalize = 
                (from clocking in listClocking
                 from x in lstClockingPenalize
                 where clocking.Id == x.ClockingId
                 select new ClockingPenalizeDto
                 {
                    Id = x.Id,
                    Value = x.Value,
                    ShiftId = clocking.ShiftId,
                    TimesCount = x.TimesCount,
                    ClockingPenalizeCreated = clocking.StartTime,
                    ClockingId = x.ClockingId,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    EmployeeId = x.EmployeeId,
                    TenantId = x.TenantId,
                    DeletedBy = x.DeletedBy,
                    PenalizeId = x.PenalizeId,
                    DeletedDate = x.DeletedDate,
                    IsDeleted = x.IsDeleted,
                    MoneyType = x.MoneyType,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedDate = x.ModifiedDate
                 }).ToList();

            return listClockingPenalize;
        }

        /// <summary>
        /// Hàm đếm  số ngày nghỉ không phép của nhân viên theo paysheetIds
        /// </summary>
        /// <param name="paySheetIds"></param>
        /// <param name="payslipIds"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<List<KeyValuePair<long, int>>> GetClockingUnAuthorizeAbsenceByPaySheetIds(List<long> paySheetIds, List<long> payslipIds, long employeeId)
        {
            var tenantId = AuthService.Context.TenantId;
            var paySlipAsQueryAble = Db.From<Payslip>()
                .Join<Paysheet>((ps, p) => ps.PaysheetId == p.Id)
                .Join<Paysheet, Clocking>((p, c) => c.StartTime >= p.StartTime && c.StartTime < p.EndTime)
                .Where<Clocking>(x => x.EmployeeId == employeeId)
                .Where<Clocking>(x => x.TenantId == tenantId)
                .Where<Clocking>(c =>
                    c.ClockingStatus != (byte)ClockingStatuses.Created &&
                    c.ClockingStatus != (byte)ClockingStatuses.Void
                );

            paySlipAsQueryAble = paySlipAsQueryAble.Where<Paysheet>(x => paySheetIds.Contains(x.Id))
                .Where<Payslip>(x => payslipIds.Contains(x.Id))
                .Where<Payslip>(x => x.TenantId == tenantId)
                .Where<Payslip>(x => x.EmployeeId == employeeId)
                .Select<Payslip, Clocking>((p, c) => new
                {
                    PayslipId = p.Id,
                    c.Id,
                    c.AbsenceType,
                    c.ClockingStatus,
                    c.CheckInDate,
                    c.CheckOutDate,
                    c.StartTime,
                    c.EndTime

                });

            var result = await Db.SelectAsync<PayslipClockingDto>(paySlipAsQueryAble);

            var clockingPaySlipKeyValuePairs = result?.GroupBy(x => x.PayslipId)
                .Select(grb =>
                    new KeyValuePair<long, int>
                    (
                        grb.Key,
                        grb.GroupBy(cl => cl.StartTime.Date)
                           .Select(x => new
                           {
                               DateKey = x.Key,
                               CountUnAuthor = x.Any(cl => cl.AbsenceType != (byte)AbsenceTypes.UnauthorisedAbsence) ? 0 : 1
                           })
                           .Sum(x => x.CountUnAuthor)
                    )
                ).ToList();

            return clockingPaySlipKeyValuePairs;
        }

        /// <summary>
        /// Hàm đếm  số ngày nghỉ không phép của nhân viên
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        public async Task<List<KeyValuePair<long, int>>> GetClockingUnAuthorizedAbsence(DateTime from, DateTime to, List<long> employeeIds)
        {
            var tenantId = AuthService.Context.TenantId;
            var toDate = to.Date.AddDays(1);
            var clockingAsQueryAble = Db.From<Clocking>()
                .Where<Clocking>(c => c.TenantId == tenantId)
                .Where<Clocking>(c => employeeIds.Contains(c.EmployeeId))
                .Where<Clocking>(c =>
                    c.StartTime >= from && c.StartTime < toDate &&
                    c.ClockingStatus != (byte)ClockingStatuses.Created &&
                    c.ClockingStatus != (byte)ClockingStatuses.Void
                );

            var result = await Db.SelectAsync<ClockingDto>(clockingAsQueryAble);

            var clockingKeyValuePairs = result?.GroupBy(x => x.EmployeeId)
                .Select(grb =>
                    new KeyValuePair<long, int>
                    (
                        grb.Key,
                        grb.GroupBy(cl => cl.StartTime.Date)
                            .Select(x => new
                            {
                                DateKey = x.Key,
                                CountUnAuthor = x.Any(cl => cl.AbsenceType != (byte)AbsenceTypes.UnauthorisedAbsence) ? 0 : 1
                            })
                            .Sum(x => x.CountUnAuthor)
                    )
                ).ToList();
            return clockingKeyValuePairs;
        }

        public async Task<List<ClockingDto>> GetClockingsForClockingGps(int branchId, long employeeId)
        {
            var now = DateTime.Now;
            var startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            var endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

            var shiftsExp = Db.From<Shift>()
                .Where(x => !x.IsDeleted)
                .Where(x => x.BranchId == branchId)
                .Where(x => x.IsActive)
                .Where(x => x.TenantId == AuthService.Context.TenantId);
            var shifts = await Db.SelectAsync(shiftsExp);

            if (shifts == null || !shifts.Any())
            {
                return new List<ClockingDto>();
            }

            var clockingExp = Db.From<Clocking>()
            .Where(x => x.TenantId == AuthService.Context.TenantId)
            .Where(x => !x.IsDeleted)
            .Where(x => x.BranchId == branchId)
            .Where(x => x.ClockingStatus != (byte)ClockingStatuses.Void)
            .Where(x => x.StartTime >= startTime && x.StartTime <= endTime)
            .Where(x => shifts.Select(s => s.Id).ToList().Contains(x.ShiftId))
            .Where(x => x.EmployeeId == employeeId);

            var clockings = await Db.SelectAsync<ClockingDto>(clockingExp);

            if (clockings == null || !clockings.Any()) return clockings;

            var queryTimeSheet = Db.From<TimeSheet>().Join<TimeSheet, Clocking>((ts, cl) => ts.Id == cl.TimeSheetId)
                .Where<Clocking>(x => x.TenantId == AuthService.Context.TenantId)
                .Where<Clocking>(x => x.BranchId == branchId)
                .Where<Clocking>(x => shifts.Select(s => s.Id).ToList().Contains(x.ShiftId))
                .Where<Clocking>(x => x.EmployeeId == employeeId)
                .Where<Clocking>(x => x.ClockingStatus != (byte)ClockingStatuses.Void)
                .Where<Clocking> (x => x.StartTime >= startTime && x.StartTime <= endTime)
                .Where<TimeSheet>(x => x.TenantId == AuthService.Context.TenantId);

            var timeSheets = await Db.SelectAsync(queryTimeSheet);

            //Clocking has shift deleted but has not CheckedIn, CheckedOut or check WorkOff yet
            //We won't get it
            var listClockingStatus = new List<byte>()
            {
                (byte)ClockingStatuses.CheckedIn,
                (byte)ClockingStatuses.CheckedOut,
                (byte)ClockingStatuses.WorkOff
            };

            var clockingDtoFromSource =
                clockings.Where(cl => listClockingStatus.Contains(cl.ClockingStatus))
                           .GroupBy(x => x.ShiftId)
                           .Select(x => x.Key)
                           .ToList();

            var queryShiftIsDelete =
                Db.From<Shift>()
                    .Where(x => x.TenantId == AuthService.Context.TenantId)
                    .Where(x => !x.IsDeleted)
                    .Where(x => !x.IsActive)
                    .Where(x => x.BranchId == branchId);

            var shiftIsDelete = await Db.SelectAsync(queryShiftIsDelete);

            var shiftIdShouldNotExists =
                shiftIsDelete.Where(x => !clockingDtoFromSource.Contains(x.Id))
                    .GroupBy(x => x.Id)
                    .Select(x => x.Key)
                    .ToList();

            if (shiftIdShouldNotExists.Any())
            {
                clockings = clockings.Where(x => !shiftIdShouldNotExists.Contains(x.ShiftId)).ToList();
            }

            var shiftIds = clockings.Select(x => x.ShiftId).ToList();

            var shiftsForClockingsQuery = Db.From<Shift>()
                    .Where(x => shiftIds.Contains(x.Id))
                    .Where(x => x.TenantId == AuthService.Context.TenantId);

            var shiftsForClockings = await Db.SelectAsync(shiftsForClockingsQuery);

            var clockingIds = clockings.Select(x => x.Id).ToList();
            var clockingHistoriesQuery = Db.From<ClockingHistory>()
                .Where(x => clockingIds.Contains(x.ClockingId))
                .Where(x => x.TenantId == AuthService.Context.TenantId);
            var clockingHistories = await Db.SelectAsync(clockingHistoriesQuery);

            foreach (var clocking in clockings)
            {
                clocking.TimeSheet = _mapper.Map<TimeSheetDto>(timeSheets.FirstOrDefault(x => x.Id == clocking.TimeSheetId));
                clocking.ClockingHistories = clockingHistories.Where(x => x.ClockingId == clocking.Id).ToList();
                clocking.Shift = _mapper.Map<ShiftDto>(shiftsForClockings.FirstOrDefault(x => x.Id == clocking.ShiftId));
            }

            return clockings;
        }

        public async Task<List<Clocking>> GetClockingBySpecificationForClockingGpsAsync(ISpecification<Clocking> spec, bool includeSoftDelete = false)
        {
            var filterSoftDelete = new FilterSoftDeleteSpec<Clocking>();

            // Soft delete
            if (!_authService.WithDeleted && typeof(Clocking).HasInterface(typeof(ISoftDelete)) && !includeSoftDelete)
            {
                spec = spec.And(filterSoftDelete);
            }

            return await Db.SelectAsync(spec.GetExpression());
        }
    }
}
