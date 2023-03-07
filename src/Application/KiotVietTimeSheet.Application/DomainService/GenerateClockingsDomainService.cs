using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class GenerateClockingsDomainService : IGenerateClockingsDomainService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IBranchSettingReadOnlyRepository _branchSettingReadOnlyRepository;
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IEmployeeBranchReadOnlyRepository _employeeBranchReadOnlyRepository;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IAuthService _authService;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;


        private readonly Dictionary<long, List<Clocking>> _dictionaryEmployeeNewClockings = new Dictionary<long, List<Clocking>>();
        private readonly List<ClockingDto> _clockingsOverlapTime = new List<ClockingDto>();
        private readonly List<Clocking> _clockingNeedUpdate = new List<Clocking>();
        private readonly List<long> _validClockingIds = new List<long>();
        private readonly List<Clocking> _timeSheetClockings = new List<Clocking>();

        private GenerateClockingByType _generateByType;
        #endregion

        #region Constructors
        public GenerateClockingsDomainService(
            IMapper mapper,
            IBranchSettingReadOnlyRepository branchSettingReadOnlyRepository,
            IHolidayReadOnlyRepository holidayReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IEmployeeBranchReadOnlyRepository employeeBranchReadOnlyRepository,
            IApplicationConfiguration applicationConfiguration,
            IAuthService authService,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository)
        {
            _mapper = mapper;
            _branchSettingReadOnlyRepository = branchSettingReadOnlyRepository;
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _employeeBranchReadOnlyRepository = employeeBranchReadOnlyRepository;
            _applicationConfiguration = applicationConfiguration;
            _authService = authService;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
        }
        #endregion

        #region Public methods
        public async Task<GenerateClockingsDomainServiceDto> GenerateClockingForTimeSheets(GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto)
        {
            var result = new GenerateClockingsDomainServiceDto();

            if (generateClockingForTimeSheetsDto?.GenerateByType == null) return result;

            _generateByType = (GenerateClockingByType)generateClockingForTimeSheetsDto.GenerateByType;
            result = await CheckValidateGeneralTimeSheet(_generateByType, generateClockingForTimeSheetsDto);
            if (!result.IsValid) return result;

            if (generateClockingForTimeSheetsDto.TimeSheets != null && generateClockingForTimeSheetsDto.TimeSheets.Any())
            {
                // Lấy ca làm việc bao gồm trường hợp xóa clocking lấy cả ca làm việc ngừng HĐ
                var shifts = await GetShiftsFromTimeSheets(generateClockingForTimeSheetsDto);

                if (shifts.Any())
                {
                    // Lấy chi tiết làm việc của các lịch làm việc sẽ cập nhật
                    var listClocking = new List<Clocking>();
                    if (generateClockingForTimeSheetsDto.TimeSheets.Any(x => x.Id > 0))
                    {
                        var timeSheetIds = generateClockingForTimeSheetsDto.TimeSheets.Select(x => x.Id).Distinct().ToList();
                        listClocking =
                            await _clockingWriteOnlyRepository.GetBySpecificationAsync(
                                new FindClockingByTimeSheetIdsSpec(timeSheetIds).Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)));
                    }

                    // Lấy ngày nghỉ lễ tết và ngày làm việc của chi nhánh
                    var daysOffInSystem = await GetDaysOffInSystem(generateClockingForTimeSheetsDto.TimeSheets,
                        generateClockingForTimeSheetsDto.Holidays, generateClockingForTimeSheetsDto.WorkingDays);

                    // Lấy chi tiết làm việc đang tồn tại trong hệ thống
                    var listClockingInSystem =
                        await GetListClockingByEmployeesTimeSheets(generateClockingForTimeSheetsDto.TimeSheets);

                    //Lấy danh sách ngày làm việc thay đổi của chi nhánh
                    var listDayOfWeeksChanged = new List<DayOfWeek>();
                    if (generateClockingForTimeSheetsDto.WorkingDaysChanged != null && generateClockingForTimeSheetsDto.WorkingDaysChanged.Any())
                    {
                        listDayOfWeeksChanged = (from g in generateClockingForTimeSheetsDto.WorkingDaysChanged
                                                 select (DayOfWeek)g).ToList();
                    }

                    GetAndCancelClockingTimeSheet(
                        generateClockingForTimeSheetsDto,
                        listClocking,
                        listClockingInSystem,
                        shifts,
                        daysOffInSystem,
                        listDayOfWeeksChanged);
                }
            }

            if (_timeSheetClockings.Count(c => c.Id <= 0) + _clockingNeedUpdate.Count > Constant.MaximumClockingCanUpdate)
            {
                result.IsValid = false;
                result.ValidationErrors = new List<string> { string.Format(Message.clocking_maximumClockingUpdate, Constant.MaximumClockingCanUpdate) };
                return result;
            }

            result.IsValid = true;
            result.TimeSheets = generateClockingForTimeSheetsDto?.TimeSheets;
            result.ClockingsOverlapTime = _clockingsOverlapTime;
            result.ClockingNeedUpdateIds = _clockingNeedUpdate.Select(x => x.Id).ToList();
            result.TimeSheetClockings = _timeSheetClockings;
            return result;
        }

        private async Task<GenerateClockingsDomainServiceDto> CheckValidateGeneralTimeSheet(GenerateClockingByType generateByType, GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto)
        {
            var result = new GenerateClockingsDomainServiceDto
            {
                IsValid = true
            };

            if (generateClockingForTimeSheetsDto.TimeSheets == null || !generateClockingForTimeSheetsDto.TimeSheets.Any())
            {
                return result;
            }

            if (generateByType != GenerateClockingByType.TimeSheet) return result;

            // Validate 'Ngày kết thúc' khi có tự động tạo lịch (AutoGenerateClocking)
            // Không validate background service tạo tự động
            if (!_authService.Context.IsBackgroundService)
            {
                var validateEndDateResult = await GetExtendValidEndDate(generateClockingForTimeSheetsDto.TimeSheets);
                if (validateEndDateResult != null)
                {
                    result.IsValid = false;
                    result.ValidationErrors = new List<string>{ validateEndDateResult.ErrorMessage };

                    return result;
                }
            }

            var employeeIds = generateClockingForTimeSheetsDto.TimeSheets.Select(x => x.EmployeeId).ToList();

            // Lấy danh sách chi nhánh làm việc của nhân viên
            var listBranchWorking =
                await _employeeBranchReadOnlyRepository.GetBySpecificationAsync(
                    new FindBranchByEmployeeIdsSpec(employeeIds));

            var validationResult = await (new GeneralTimeSheetValidator(_employeeReadOnlyRepository, _shiftReadOnlyRepository, listBranchWorking, _applicationConfiguration.TimeSheetValidate)).ValidateAsync(generateClockingForTimeSheetsDto.TimeSheets);
            if (validationResult.IsValid) return result;

            result.IsValid = false;
            result.ValidationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return result;
        }

        private async Task<ValidationFailure> GetExtendValidEndDate( List<TimeSheet> timeSheets)
        {
            foreach (var timeSheet in timeSheets)
            {
                var isValid = await IsExtendValidEndDate(timeSheet);
                if (!isValid) return new ValidationFailure(nameof(timeSheet.EndDate), Message.timeSheet_maximumBookCalendar.Fmt(_applicationConfiguration.TimeSheetValidate.AllowOrderMaxMonth));
            }

            return null;
        }

        private async Task<bool> IsExtendValidEndDate(TimeSheet timeSheet)
        {
            var totalDay = (int)(timeSheet.EndDate.Date - timeSheet.StartDate.Date).TotalDays;
            if (totalDay <= _applicationConfiguration.TimeSheetValidate.AllowOrderMaxDay) return true;

            var timeSheetExist = await _timeSheetWriteOnlyRepository.FindByIdAsync(timeSheet.Id);
            if (
                (timeSheetExist == null && totalDay > _applicationConfiguration.TimeSheetValidate.AllowOrderMaxDay)
                ||
                (timeSheetExist != null && timeSheetExist.EndDate.Date < timeSheet.EndDate.Date))
            {
                return false;
            }

            return true;
        }

        private void GetAndCancelClockingTimeSheet(
            GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto,
            List<Clocking> listClocking,
            List<Clocking> listClockingInSystem,
            List<Shift> shifts,
            DaysOffDto daysOffInSystem,
            List<DayOfWeek> listDayOfWeeksChanged)
        {
            var timeSheetIdTemp = 0;

            // Sắp xếp lịch làm việc theo ngày bắt đầu
            generateClockingForTimeSheetsDto.TimeSheets = generateClockingForTimeSheetsDto.TimeSheets.OrderBy(x => x.StartDate).ToList();

            var isExistsGenerateClockingApplyTimes = generateClockingForTimeSheetsDto.ApplyTimes?.Any() ?? false;

            if (isExistsGenerateClockingApplyTimes)
                generateClockingForTimeSheetsDto.ApplyTimes = generateClockingForTimeSheetsDto.ApplyTimes.OrderBy(x => x.From).ToList();

            foreach (var timeSheet in generateClockingForTimeSheetsDto.TimeSheets)
            {
                // tempId để map clocking vs timeSheet
                if (timeSheet.Id <= 0)
                    timeSheet.TemporaryId = --timeSheetIdTemp;

                var timeSheetOnShifts = DivideTimeSheetByShift(timeSheet, listClocking);

                if (!timeSheetOnShifts.Any()) continue;

                //Loại bỏ ngày làm việc không liên quan đến ca cập nhập nếu trường hợp thay đổi lịch làm việc
                if (generateClockingForTimeSheetsDto.GenerateByType == GenerateClockingByType.TimeSheet)
                {
                    var listClockingFromTimeSheet = listClocking.Where(x => x.TimeSheetId == timeSheet.Id).ToList();
                    var listClockingNeedCancel = CancelTimeSheetIfUnRelateShift(timeSheet, timeSheetOnShifts, listClockingFromTimeSheet);
                    var clockingNeedCancelIds = listClockingNeedCancel.Select(c => c.Id).ToList();
                    listClockingInSystem = listClockingInSystem.Where(x => !clockingNeedCancelIds.Contains(x.Id)).ToList();
                }

                foreach (var timeSheetOnShift in timeSheetOnShifts)
                {
                    var shift = shifts.Find(x => x.Id == timeSheetOnShift.ShiftId);

                    CreateOrCancelClocking(
                        shift, timeSheet, timeSheetOnShift, daysOffInSystem,
                        listClockingInSystem, listDayOfWeeksChanged, 
                        isExistsGenerateClockingApplyTimes, generateClockingForTimeSheetsDto);

                    if (!timeSheetOnShift.Clockings.Any()) continue;
                    _timeSheetClockings.AddRange(timeSheetOnShift.Clockings);
                }
                // Hủy lịch làm việc nếu không có chi tiết làm viêc nào hoạt động
                CancelTimeSheetIfTimeSheetHasNoListClocking(timeSheet);
            }
        }

        private void CreateOrCancelClocking(
            Shift shift, TimeSheet timeSheet, 
            TimeSheetOnShift timeSheetOnShift,  DaysOffDto daysOffInSystem, 
            List<Clocking> listClockingInSystem, List<DayOfWeek> listDayOfWeeksChanged, 
            bool isExistsGenerateClockingApplyTimes, GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto)
        {
            if (isExistsGenerateClockingApplyTimes)
            {
                foreach (var applyTime in generateClockingForTimeSheetsDto.ApplyTimes)
                {
                    CreateClockingForTimeSheetShift(
                        shift, timeSheet, timeSheetOnShift,
                        applyTime, daysOffInSystem, listClockingInSystem,
                        listDayOfWeeksChanged, generateClockingForTimeSheetsDto.IsAddClockings);

                    if (!generateClockingForTimeSheetsDto.IsRemoveClockings) continue;

                    var deleteFrom = applyTime?.From ?? timeSheet.StartDate;
                    var deleteTo = applyTime?.To ?? timeSheet.EndDate;
                    CancelClockingsInDaysOff(timeSheet, timeSheetOnShift, daysOffInSystem, deleteFrom, deleteTo, listDayOfWeeksChanged);
                }
            }
            else
            {
                CreateClockingForTimeSheetShift(
                    shift, timeSheet, timeSheetOnShift,
                    null, daysOffInSystem, listClockingInSystem,
                    listDayOfWeeksChanged, generateClockingForTimeSheetsDto.IsAddClockings);
            }
        }

        /// <summary>
        /// Tạo chi tiết làm viêc cho ca làm việc trong lịch làm việc và 
        /// hủy chi tiết làm việc không đủ điều kiện
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="applyTime"></param>
        /// <param name="daysOffInSystem"></param>
        /// <param name="listClockingInSystem"></param>
        /// <param name="listDayOfWeeksChanged"></param>
        /// <param name="isAddClocking"></param>
        private void CreateClockingForTimeSheetShift(
            Shift shift, TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, DateRangeDto applyTime,
            DaysOffDto daysOffInSystem, List<Clocking> listClockingInSystem, List<DayOfWeek> listDayOfWeeksChanged,
            bool isAddClocking)
        {
            if (shift == null) return;

            if (!isAddClocking) return;

            var range = GetRangeGenerate(timeSheet, applyTime);

            if (range == null) return;

            // tạo chi tiết làm viêc cho ca làm việc trong lịch làm việc
            GenerateListClocking(shift, timeSheet, timeSheetOnShift, range.From, range.To, listDayOfWeeksChanged);

            // hủy chi tiết làm việc không đủ điều kiện
            RemoveInValidClockingsAfterGenerate(timeSheet, timeSheetOnShift, daysOffInSystem,
                listClockingInSystem, range, listDayOfWeeksChanged);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tạo chi tiết làm việc cho từng lich làm việc
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="applyFrom"></param>
        /// <param name="applyTo"></param>
        /// <param name="listDayOfWeeksChanged"></param>
        private void GenerateListClocking(Shift shift, TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, DateTime applyFrom, DateTime applyTo, List<DayOfWeek> listDayOfWeeksChanged)
        {
            if (timeSheetOnShift.Clockings == null)
                timeSheetOnShift.Clockings = new List<Clocking>(); 

            var listShiftClocking = timeSheetOnShift.Clockings;
            if (timeSheet.IsRepeat == true)
            {
                GenerateClockingRepeated(timeSheet, timeSheetOnShift, applyFrom, applyTo, listDayOfWeeksChanged, shift, listShiftClocking);
                return;
            }
            
            // Không tạo mới chi tiết làm việc cho các lịch làm việc không lặp lại(1 ngày) khi thêm, sửa, xóa ngày làm việc chi nhánh và lẽ tết
            if (_generateByType == GenerateClockingByType.BranchWorkingDay || _generateByType == GenerateClockingByType.Holiday) return;

            if (applyFrom != default(DateTime) && timeSheet.StartDate < applyFrom) return;

            var clocking = Clocking.CreateForTimeSheet(timeSheet, timeSheetOnShift.Clockings, shift, timeSheet.StartDate);
            AddNewOrAddValidClocking(listShiftClocking, clocking);
        }

        private void GenerateClockingRepeated(
            TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, 
            DateTime applyFrom, DateTime applyTo,
            List<DayOfWeek> listDayOfWeeksChanged, Shift shift,
            List<Clocking> listShiftClocking)
        {
            //Tạo lich làm việc trong trường hợp cách tuần hoặc cách ngày
            var timeSheetStart = timeSheet.StartDate;
            var start = applyFrom;
            var end = applyTo;
            var startDayOfWeek = (byte)start.DayOfWeek;
            var firstWeek = GetFirstWeek(timeSheetOnShift, startDayOfWeek, start);
            byte defaultDailyRepeatDay = 1;
            while (start <= end)
            {
                //Nếu người dùng chọn ngày để thêm?
                if (listDayOfWeeksChanged.Any() && !CheckDayClockingExistInDayOfChange(listDayOfWeeksChanged, start))
                {
                    start = start.AddDays(1);
                    continue;
                }
                startDayOfWeek = (byte)start.DayOfWeek;
                if (timeSheet.RepeatType == (byte)RepeatTypes.Weekly)
                {
                    CaseAddClockingRepeatWeekly(timeSheet, timeSheetOnShift, startDayOfWeek, firstWeek, start, listShiftClocking, shift);
                    start = start.AddDays(1);
                    continue;
                }

                // lặp lại hàng ngày
                if (timeSheet.RepeatEachDay == null) timeSheet.SetRepeatEachDay(defaultDailyRepeatDay);

                if ((start - timeSheetStart).Days % timeSheet.RepeatEachDay == 0)
                {
                    var clocking = Clocking.CreateForTimeSheet(timeSheet, timeSheetOnShift.Clockings, shift, start);
                    AddNewOrAddValidClocking(listShiftClocking, clocking);

                    if (timeSheet.RepeatEachDay != null)
                        start = start.AddDays((double)timeSheet.RepeatEachDay);
                }
                else start = start.AddDays(1);
            }
        }

        private void CaseAddClockingRepeatWeekly(
            TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, 
            byte startDayOfWeek, List<DateTime> firstWeek,
            DateTime start, List<Clocking> listShiftClocking, Shift shift)
        {
            if (timeSheetOnShift.WorkingDayOfWeek == null || !timeSheetOnShift.WorkingDayOfWeek.Contains(startDayOfWeek)) return;

            // xác định ngày có thể thêm trong trường hợp cách tuần.
            var dayInFirstWeek =
                firstWeek.FirstOrDefault(day => (byte)day.DayOfWeek == startDayOfWeek);
            var canGenerate = Math.Floor((decimal)(start.Subtract(dayInFirstWeek).TotalDays + 1) / 7) %
                timeSheet.RepeatEachDay == 0;

            if (!canGenerate) return;

            var clocking = Clocking.CreateForTimeSheet(timeSheet, timeSheetOnShift.Clockings, shift, start);
            AddNewOrAddValidClocking(listShiftClocking, clocking);
        }

        private void AddNewOrAddValidClocking(List<Clocking> listShiftClocking, Clocking clocking)
        {
            if (clocking == null) return;

            if (clocking.Id > 0) _validClockingIds.Add(clocking.Id);
            else AddClocking(listShiftClocking, clocking);
        }

        private async Task<List<Shift>> GetShiftsFromTimeSheets(GenerateClockingForTimeSheetsDto generateClockingForTimeSheetsDto)
        {
            var shiftIds = generateClockingForTimeSheetsDto.TimeSheets.SelectMany(x => x.TimeSheetShifts)
                .SelectMany(x => x.ShiftIdsToList).Distinct().ToList();

            ISpecification<Shift> shiftSpecification = new FindShiftsByIds(shiftIds);

            // Trường hợp xóa clocking lấy cả ca làm việc ngừng HĐ
            if (!generateClockingForTimeSheetsDto.IsRemoveClockings)
                shiftSpecification = shiftSpecification.And(new FindShiftActiveSpec());

            var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(shiftSpecification);

            return shifts ?? new List<Shift>();
        }

        private List<DateTime> GetFirstWeek(TimeSheetOnShift timeSheetOnShift, byte startDayOfWeek, DateTime start)
        {
            var firstWeek = new List<DateTime>();
            if (timeSheetOnShift.WorkingDayOfWeek == null) return firstWeek;

            foreach (var item in timeSheetOnShift.WorkingDayOfWeek)
            {
                if (item == 0)
                {
                    // trường hợp ngày chủ nhât
                    if (startDayOfWeek != 0)
                    {
                        firstWeek.Add(start.AddDays(7 - startDayOfWeek));
                    }

                    if (startDayOfWeek == 0)
                    {
                        firstWeek.Add(start);
                    }

                    continue;
                }

                if (item < startDayOfWeek) firstWeek.Add(start.AddDays(-(startDayOfWeek - item)));
                else if (item > startDayOfWeek) firstWeek.Add(start.AddDays(item - startDayOfWeek));
                else firstWeek.Add(start);
            }

            return firstWeek;
        }

        /// <summary>
        /// Hủy các chi tiết làm việc vi phạm
        /// Xóa mềm các chi tiết làm việc đc tạo trùng
        /// Nếu không có chi tiết làm việc nào hợp lệ sẽ hủy lịch làm việc
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="daysOff"></param>
        /// <param name="clockingsInSystem"></param>
        /// <param name="applyRange"></param>
        /// <param name="listDayOfWeeksChanged"></param>
        /// <returns></returns>
        private void RemoveInValidClockingsAfterGenerate(TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, DaysOffDto daysOff, List<Clocking> clockingsInSystem, DateRange applyRange, List<DayOfWeek> listDayOfWeeksChanged)
        {
            // Xóa mềm các chi tiết làm việc đc tạo trùng
            for (var i = timeSheetOnShift.Clockings.Count - 1; i >= 0; i--)
            {
                SoftDeleteInValidListClocking(timeSheetOnShift.Clockings[i]);

                RemoveOverlapListClocking(timeSheetOnShift, timeSheetOnShift.Clockings[i], clockingsInSystem);
            }
            // Hủy chi tiết làm việc trùng nghỉ lễ hoặc ngày nghỉ của chi nhánh
            CancelClockingsInDaysOff(timeSheet, timeSheetOnShift, daysOff, applyRange.From, applyRange.To, listDayOfWeeksChanged);

        }

        /// <summary>
        /// Kiểm tra và hủy lịch làm việc không có chi tiết làm việc
        /// </summary>
        /// <param name="timeSheet"></param>
        private void CancelTimeSheetIfTimeSheetHasNoListClocking(TimeSheet timeSheet)
        {
            var isActiveClocking =
                _timeSheetClockings.Any(x =>
                    x.TimeSheetId == (timeSheet.Id > 0 ? timeSheet.Id : timeSheet.TemporaryId) && !x.IsDeleted &&
                    x.ClockingStatus != (byte)ClockingStatuses.Void);

            if (isActiveClocking) return;

            if (timeSheet.Id <= 0)
            {
                timeSheet.UpdateTimeSheetStatus((byte)TimeSheetStatuses.Void);
            }
            else
            {
                timeSheet.Cancel();
            }
        }

        private List<Clocking> CancelTimeSheetIfUnRelateShift(TimeSheet timeSheet, List<TimeSheetOnShift> timeSheetOnShifts, List<Clocking> listClocking)
        {
            if (timeSheet.Id <= 0)
                return new List<Clocking>();

            var currentShiftIds = timeSheetOnShifts.Select(ts => ts.ShiftId).ToList();
            listClocking = listClocking.Where(c => currentShiftIds.All(s => s != c.ShiftId) && c.ClockingStatus == (byte)ClockingStatuses.Created).ToList();
            listClocking.ForEach(c => c.Reject());
            _timeSheetClockings.AddRange(listClocking);
            _clockingNeedUpdate.AddRange(listClocking);
            if (!timeSheetOnShifts.Any())
            {
                CancelTimeSheetIfTimeSheetHasNoListClocking(timeSheet);
            }

            return listClocking;
        }

        /// <summary>
        /// Lấy khoảng thời gian được áp dụng để tạo lịch làm việc
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        private DateRange GetRangeGenerate(TimeSheet timeSheet, DateRangeDto dateRange)
        {
            var from = timeSheet.StartDate;
            var to = timeSheet.EndDate;

            if (dateRange?.From != null && dateRange.From > timeSheet.StartDate)
            {
                from = (DateTime)dateRange.From;
            }

            if (dateRange?.To != null && dateRange.To < timeSheet.EndDate)
            {
                to = (DateTime)dateRange.To;
            }

            return new DateRange()
            {
                From = from.Date,
                To = to
            };
        }

        /// <summary>
        /// Lấy ngày nghỉ lễ tết và ngày làm việc của chi nhánh
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="holidays"></param>
        /// <param name="workingDays"></param>
        /// <returns></returns>
        private async Task<DaysOffDto> GetDaysOffInSystem(List<TimeSheet> timeSheet, List<Holiday> holidays = null,
            List<byte> workingDays = null)
        {
            var branchWorkingDays = new List<BranchWorkingDayDto>();
            if (holidays == null)
            {
                holidays = await _holidayReadOnlyRepository.GetAllAsync();
            }

            if (workingDays == null)
            {
                // Trường hợp update timesheet, holiday
                var branchIds = timeSheet.Select(x => x.BranchId).Distinct().ToList();
                var specifications = new FindBranchSettingByBranchIdsSpec(branchIds);
                var branchSettings = await _branchSettingReadOnlyRepository.GetBySpecificationAsync(specifications);
                if (branchSettings != null && branchSettings.Any())
                {

                    foreach (var branchSetting in branchSettings)
                    {
                        branchWorkingDays.Add(new BranchWorkingDayDto
                        {
                            BranchId = branchSetting.BranchId,
                            WorkingDays = branchSetting.WorkingDaysInArray.ToList()
                        });
                    }

                }
            }
            else
            {
                // Trường hợp update branchSetting se kèm theo workingDays mới đc update
                if (timeSheet != null && timeSheet.Any())
                {
                    var branchId = timeSheet.FirstOrDefault()?.BranchId;
                    if (branchId != null)
                        branchWorkingDays.Add(new BranchWorkingDayDto
                        {
                            BranchId = (long)branchId,
                            WorkingDays = workingDays
                        });
                }

            }
            return new DaysOffDto { Holidays = holidays, BranchWorkingDays = branchWorkingDays };
        }

        /// <summary>
        /// Hủy clocking trùng nghỉ lễ hoắc ngày nghỉ của chi nhành
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="daysOff"></param>
        /// <param name="applyFrom"></param>
        /// <param name="applyTo"></param>
        /// <param name="listDayOfWeeksChanged"></param>
        /// <returns></returns>
        private void CancelClockingsInDaysOff(TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, DaysOffDto daysOff, DateTime applyFrom, DateTime applyTo, List<DayOfWeek> listDayOfWeeksChanged)
        {
            // Khi thêm/sửa lịch lẻ, có thể đặt CTLV vào ngày nghỉ hoặc lễ tết
            if (timeSheet.IsRepeat != true && _generateByType == GenerateClockingByType.TimeSheet)
                return;

            for (var clockingIndex = timeSheetOnShift.Clockings.Count - 1; clockingIndex >= 0; clockingIndex--)
            {
                // Kiểm tra clocking có trong thời gian đc phép áp dụng
                if (!IsTimeInTimeRange(timeSheetOnShift.Clockings[clockingIndex].StartTime.Date, applyFrom.Date, applyTo.Date) || timeSheetOnShift.Clockings[clockingIndex].ClockingStatus != (byte)ClockingStatuses.Created)
                    continue;
                
                //Nếu người dùng chọn ngày để hủy. Loại clocking không tồn tại trong ngày được chọn ?
                if (listDayOfWeeksChanged.Any() && !CheckDayClockingExistInDayOfChange(listDayOfWeeksChanged, timeSheetOnShift.Clockings[clockingIndex].StartTime.Date))
                    continue;
                
                if (!AddOrRemoveClockingDayOffAbsence(timeSheet, timeSheetOnShift, daysOff, clockingIndex))
                    continue;

                AddOrRemoveWhenWorkingDayIsNull(timeSheet, timeSheetOnShift, daysOff, clockingIndex);
            }
        }

        /// <summary>
        /// Loại các clockings không cho phép đặt lịch trên ngày nghỉ lễ Tết
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="daysOff"></param>
        /// <param name="clockingIndex"></param>
        /// <returns></returns>
        private bool AddOrRemoveClockingDayOffAbsence(TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift,
            DaysOffDto daysOff, int clockingIndex)
        {
            if (timeSheet.SaveOnHoliday) return true;

            var overlapHoliday = daysOff.Holidays?.FirstOrDefault(holiday => IsTimeInTimeRange(timeSheetOnShift.Clockings[clockingIndex].StartTime.Date, holiday.From.Date, holiday.To.Date));
            if (overlapHoliday == null) return true;

            var canceledClockingDto = _mapper.Map<ClockingDto>(timeSheetOnShift.Clockings[clockingIndex]);
            canceledClockingDto.OverlapType = (byte)OverlapType.Holiday;
            canceledClockingDto.OverlapHolidayName = overlapHoliday.Name;

            //Chỉ hủy các chi tiết làm việc đang có khi tạo, sửa nghỉ lễ hoặc lịch làm việc
            if (timeSheetOnShift.Clockings[clockingIndex].Id > 0 && (_generateByType == GenerateClockingByType.Holiday || _generateByType == GenerateClockingByType.TimeSheet))
            {
                _clockingsOverlapTime.Add(canceledClockingDto);
                timeSheetOnShift.Clockings[clockingIndex].Reject();
                _clockingNeedUpdate.Add(timeSheetOnShift.Clockings[clockingIndex]);
                return false;
            }

            // Chỉ cancel chi tiết làm việc trong trường hợp tạo mới lịch làm việc lặp lại
            if (timeSheet.IsRepeat != true || timeSheetOnShift.Clockings[clockingIndex].Id != 0) return true;

            _dictionaryEmployeeNewClockings[timeSheetOnShift.Clockings[clockingIndex].EmployeeId].Remove(timeSheetOnShift.Clockings[clockingIndex]);
            _clockingsOverlapTime.Add(canceledClockingDto);
            timeSheetOnShift.Clockings.RemoveAt(clockingIndex);
            return false;

        }

        /// <summary>
        /// Loại các các chi tiết làm việc ơ các lịch làm việc làm vào các thứ không đc định trc
        /// </summary>
        /// <param name="timeSheet"></param>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="daysOff"></param>
        /// <param name="clockingIndex"></param>
        private void AddOrRemoveWhenWorkingDayIsNull(TimeSheet timeSheet, TimeSheetOnShift timeSheetOnShift, DaysOffDto daysOff, int clockingIndex)
        {
            // Loại các các chi tiết làm việc ơ các lịch làm việc làm vào các thứ không đc định trc
            if (timeSheetOnShift.WorkingDayOfWeek != null && timeSheetOnShift.WorkingDayOfWeek.Any()) return;

            var branchWorkingDay = daysOff.BranchWorkingDays.FirstOrDefault(x => x.BranchId == timeSheet.BranchId);
            if (branchWorkingDay == null || branchWorkingDay.WorkingDays.Any(w => (byte) timeSheetOnShift.Clockings[clockingIndex].StartTime.DayOfWeek == w)) return;

            var canceledClockingDto = _mapper.Map<ClockingDto>(timeSheetOnShift.Clockings[clockingIndex]);
            canceledClockingDto.OverlapType = (byte)OverlapType.BranchOffDay;

            //Chỉ hủy các chi tiết làm việc đang có khi tạo, sửa ngày làm việc chi nhánh hoặc lich làm việc
            if (timeSheetOnShift.Clockings[clockingIndex].Id > 0 && (_generateByType == GenerateClockingByType.BranchWorkingDay || _generateByType == GenerateClockingByType.TimeSheet))
            {
                _clockingsOverlapTime.Add(canceledClockingDto);
                timeSheetOnShift.Clockings[clockingIndex].Reject();
                _clockingNeedUpdate.Add(timeSheetOnShift.Clockings[clockingIndex]);
                return;
            }
            
            // Chỉ loại bỏ chi tiết làm việc trong trường hợp tạo mới lịch làm việc lặp
            if (timeSheet.IsRepeat != true || timeSheetOnShift.Clockings[clockingIndex].Id != 0) return;
            _dictionaryEmployeeNewClockings[timeSheetOnShift.Clockings[clockingIndex].EmployeeId].Remove(timeSheetOnShift.Clockings[clockingIndex]);
            _clockingsOverlapTime.Add(canceledClockingDto);
            timeSheetOnShift.Clockings.RemoveAt(clockingIndex);
        }

        /// <summary>
        /// Xóa mềm các chi tiết không hợp lệ trong lịch làm việc hiện tại
        /// </summary>
        /// <param name="clocking"></param>
        private void SoftDeleteInValidListClocking(Clocking clocking)
        {
            if (
                _generateByType != GenerateClockingByType.TimeSheet
                || _validClockingIds.Contains(clocking.Id)
                || clocking.Id <= 0
                || clocking.ClockingStatus != (byte) ClockingStatuses.Created
                || clocking.IsDeleted
            ) return;

            clocking.Delete();
            _clockingNeedUpdate.Add(clocking);
        }

        /// <summary>
        /// Loại bỏ các chi tiết làm việc mới tạo trùng với lịch làm việc khác của nhân viên
        /// </summary>
        /// <param name="timeSheetOnShift"></param>
        /// <param name="clocking"></param>
        /// <param name="clockingsInSystem"></param>
        private void RemoveOverlapListClocking(TimeSheetOnShift timeSheetOnShift, Clocking clocking, List<Clocking> clockingsInSystem)
        {
            if (clocking.Id > 0 || clockingsInSystem == null || !clockingsInSystem.Any()) return;

            var existedClocking = 
                clockingsInSystem.FirstOrDefault(x => !x.IsDeleted 
                                                          && x.EmployeeId == timeSheetOnShift.EmployeeId 
                                                          && IsOverLapTimeBetweenClockings(clocking, x));
            if (existedClocking == null) return;

            // Update ca, trạng thái, checkIn, checkOut của chỉ tiết làm việc dùng để hiển thị thông tin ngày bị trùng 
            clocking.UpdateClockingShiftAndCheckInCheckOut(existedClocking.ShiftId, existedClocking.CheckInDate, existedClocking.CheckOutDate);
            clocking.UpdateClockingStatusAndAbsenceType(existedClocking.ClockingStatus, existedClocking.AbsenceType);
            var canceledClockingDto = _mapper.Map<ClockingDto>(clocking);
            canceledClockingDto.OverlapType = (byte)OverlapType.TimeSheet;
            _clockingsOverlapTime.Add(canceledClockingDto);
            timeSheetOnShift.Clockings.Remove(clocking);
        }

        /// <summary>
        /// Lấy các chi tiết làm việc các nhân viên dựa vào khoảng thời gian làm việc của các lịch làm việc
        /// </summary>
        /// <param name="timeSheets"></param>
        /// <returns></returns>
        private async Task<List<Clocking>> GetListClockingByEmployeesTimeSheets(List<TimeSheet> timeSheets)
        {
            if (timeSheets == null || !timeSheets.Any()) return null;

            var employeeIds = timeSheets.Select(x => x.EmployeeId).ToList();
            var from = timeSheets.Min(t => t.StartDate).Date;
            var to = timeSheets.Max(t => t.EndDate).AddDays(1).Date;

            var getClockingByEmployeeIdOfDaySpec = (new FindClockingByEmployeeIdsSpec(employeeIds))
                .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(@from))
                .And(new FindClockingByStartTimeLessThanSpec(to))
                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));
            var listClocking = await _clockingReadOnlyRepository.GetBySpecificationAsync(getClockingByEmployeeIdOfDaySpec);

            return listClocking;
        }

        /// <summary>
        /// Tạo mới chi tiết làm việc cho lịch làm việc và lưu vào dictionary để check không tạo trùng chi tiết làm việc
        /// </summary>
        /// <param name="listClockingOfTimeSheet"></param>
        /// <param name="newClocking"></param>
        /// <returns></returns>
        private void AddClocking(ICollection<Clocking> listClockingOfTimeSheet, Clocking newClocking)
        {

            if (_dictionaryEmployeeNewClockings.ContainsKey(newClocking.EmployeeId))
            {
                var existedInDictionary = _dictionaryEmployeeNewClockings[newClocking.EmployeeId].Any(x =>
                    IsOverLapTimeBetweenClockings(newClocking, x));
                if (existedInDictionary) return;
                listClockingOfTimeSheet.Add(newClocking);
                _dictionaryEmployeeNewClockings[newClocking.EmployeeId].Add(newClocking);
            }
            else
            {
                listClockingOfTimeSheet.Add(newClocking);
                _dictionaryEmployeeNewClockings.Add(newClocking.EmployeeId, new List<Clocking>()
                {
                    newClocking
                });
            }
        }

        private List<TimeSheetOnShift> DivideTimeSheetByShift(TimeSheet timeSheet, List<Clocking> clockings)
        {
            var result = new List<TimeSheetOnShift>();
            if (timeSheet?.TimeSheetShifts == null || !timeSheet.TimeSheetShifts.Any()) return result;
            foreach (var timeSheetShift in timeSheet.TimeSheetShifts)
            {
                foreach (var shiftId in timeSheetShift.ShiftIdsToList)
                {
                    var checkExist = result.Find(x => x.ShiftId == shiftId);
                    if (checkExist != null)
                    {
                        if (timeSheetShift.RepeatDaysOfWeekInList.Any())
                        {
                            checkExist.WorkingDayOfWeek.AddRange(timeSheetShift.RepeatDaysOfWeekInList);
                        }
                        checkExist.WorkingDayOfWeek = checkExist.WorkingDayOfWeek.Distinct().ToList();
                    }
                    else
                    {
                        result.Add(new TimeSheetOnShift
                        {
                            ShiftId = shiftId,
                            TimeSheetId = timeSheet.Id,
                            EmployeeId = timeSheet.EmployeeId,
                            WorkingDayOfWeek = timeSheetShift.RepeatDaysOfWeekInList,
                            Clockings = clockings.Where(x => x.TimeSheetId == timeSheetShift.TimeSheetId && x.ShiftId == shiftId).ToList()
                        });
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra chi tiết làm việc có trùng với danh sách làm việc được chọn
        /// </summary>
        /// <param name="listDayOfWeeksChanged"></param>
        /// <param name="dayClocking"></param>
        /// <returns></returns>
        private bool CheckDayClockingExistInDayOfChange(List<DayOfWeek> listDayOfWeeksChanged, DateTime dayClocking)
        {
            return listDayOfWeeksChanged.Any(d => dayClocking.DayOfWeek == d);
        }
        #endregion
        private class DateRange
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }


        private class TimeSheetOnShift
        {
            public long ShiftId { get; set; }
            public long EmployeeId { get; set; }
            public long TimeSheetId { get; set; }
            public List<byte> WorkingDayOfWeek { get; set; }
            public List<Clocking> Clockings { get; set; }
        }

    }
}