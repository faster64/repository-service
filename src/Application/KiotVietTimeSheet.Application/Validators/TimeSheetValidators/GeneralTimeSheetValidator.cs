using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Validators;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.ConfigModels;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class GeneralTimeSheetValidator : BaseTimeSheetValidator<List<TimeSheet>>
    {
        #region Properties
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        #endregion

        #region Constructor
        public GeneralTimeSheetValidator(
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            List<EmployeeBranch> branchsWorking,
            TimeSheetValidateConfiguration timeSheetValidateConfiguration
            )
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            ValidateStartDate();
            ValidateEndDate(timeSheetValidateConfiguration);
            ValidateRepeatDaysOfWeek();
            ValidateEmployee(branchsWorking);
            ValidateShift(branchsWorking);

        }
        #endregion

        #region Protected methods
        protected void ValidateEmployee(List<EmployeeBranch> branchsWorking)
        {
            RuleFor(timeSheets => timeSheets)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .CustomAsync(async (timeSheets, context, token) =>
                {
                    var employees = await _employeeReadOnlyRepository.GetByListEmployeeIdAsync(timeSheets.Select(x => x.EmployeeId).Distinct().ToList());
                    if (employees == null || !employees.Any())
                    {
                        context.AddFailure(string.Format(Message.timeSheet_employeesDoNotExist, string.Join(", ", timeSheets.Select(x => x.EmployeeId))));
                    }
                    else
                    {
                        var deletedEmployees = employees.Where(e => e.IsDeleted).ToList();
                        if (deletedEmployees.Any())
                        {
                            context.AddFailure(string.Format(Message.timeSheet_employeesDoNotExist,
                                string.Join(", ", deletedEmployees.Select(e => e.Name))));
                        }

                        var checkExistBranchWorking = (from t in timeSheets
                                                           from bw in branchsWorking
                                                           where t.EmployeeId == bw.EmployeeId && t.BranchId == bw.BranchId
                                                           select 1).Any();

                        if (!checkExistBranchWorking)
                        {
                            context.AddFailure(Message
                                .timeSheet_differenceBetweenEmployeeBranchIdAndTSBranchId);
                        }
                    }
                });
        }

        protected void ValidateShift(List<EmployeeBranch> branchsWorking)
        {
            RuleFor(timeSheets => timeSheets)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .CustomAsync(async (timeSheets, context, token) =>
                {
                    var isExistsTimeSheet = timeSheets.All(x => x.TimeSheetShifts != null);
                    if (!isExistsTimeSheet)
                    {
                        context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, string.Join(", ", Label.shift)));
                        return;
                    }

                    var shiftIds = timeSheets.SelectMany(x => x.TimeSheetShifts)
                                                     .SelectMany(x => x.ShiftIdsToList)
                                                     .Distinct()
                                                     .ToList();

                    var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                        new FindShiftByShiftIdsSpec(shiftIds), false, true);

                    if (!shifts.Any())
                    {
                        context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, Label.shift));
                        return;
                    }

                    AddFailureShift(shifts, context);

                    var activeShifts = shifts.Where(s => !s.IsDeleted && s.IsActive).ToList();
                    // lấy ra danh sách các shift đc coi là overlap
                    var overlapShifts = new List<PairShiftOverLap>();
                    foreach (var shift in activeShifts)
                    {
                        var overlaps = activeShifts.Where(x => shift.Id != x.Id && IsOverLapTimeBetweenShifts(shift, x)).ToList();

                        overlapShifts = (from overlap in overlaps
                                         select new PairShiftOverLap
                                         {
                                             FirstShift = shift,
                                             SecondShift = overlap
                                         }).ToList();
                        
                    }

                    var isOverlapShifts = overlapShifts.Any();

                    foreach (var timeSheet in timeSheets)
                    {
                        // kiểm tra xem trong lịch làm việc có ca nào overlap mà cung đc tạo 1 ngày không
                        GetShiftWorkingDayFrom(timeSheet, overlapShifts, isOverlapShifts, context);
                        

                        var shiftIdsToList = timeSheet.TimeSheetShifts?.SelectMany(x => x.ShiftIdsToList).ToList() ?? new List<long>();

                        var listShiftFromTimeSheet = shifts.Where(s => shiftIdsToList.Contains(s.Id)).ToList();

                        var branchesNotGetWorking = branchsWorking.Select(x => x.BranchId).ToList();

                        if (listShiftFromTimeSheet.All(shift => branchesNotGetWorking.Contains(shift.BranchId))) continue;

                        context.AddFailure(Message.timeSheet_differenceBetweenShiftBranchIdAndTSBranchId);
                        return;
                    }
                });
        }

        private void AddFailureShift(List<Shift> shifts, CustomContext context)
        {
            var deletedShifts = shifts.Where(s => s.IsDeleted).ToList();
            var inactiveShifts = shifts.Where(s => !s.IsActive).ToList();

            if (deletedShifts.Any())
            {
                context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist,
                    string.Join(", ", deletedShifts.Select(s => s.Name))));
            }

            if (inactiveShifts.Any())
            {
                context.AddFailure(string.Format(Message.timeSheet_objectsInactive,
                    string.Join(", ", inactiveShifts.Select(s => s.Name))));
            }
        }

        private void GetShiftWorkingDayFrom(TimeSheet timeSheet, List<PairShiftOverLap> overlapShifts, bool isOverlapShifts, CustomContext context)
        {
            if (!isOverlapShifts) return;

                var groupByWorkingDays = new List<ShiftWorkingDay>();
            // validate ca trong từng lịch làm việc có trùng nhau hay không
            var groups = timeSheet.TimeSheetShifts.GroupBy(x => x.RepeatDaysOfWeekInList ?? new List<byte>()).ToList();

            foreach (var group in groups)
            {
                if (group.Key.Any())
                {
                    var timeSheetShift = group.FirstOrDefault();
                    GetShiftWorkingDayFromInGroup(group, groupByWorkingDays, timeSheetShift);
                    continue;
                }
                
                groupByWorkingDays.Add(new ShiftWorkingDay
                {
                    ShiftIds = group.FirstOrDefault()?.ShiftIdsToList
                });
            }

            if (!groupByWorkingDays.Any()) return;
            
            foreach (var overlapShift in overlapShifts)
            {
                var check = groupByWorkingDays.FirstOrDefault(x =>
                    x.ShiftIds.Contains(overlapShift.FirstShift.Id) &&
                    x.ShiftIds.Contains(overlapShift.SecondShift.Id));

                if (check == null) continue;

                var workingNameDay = $"Chủ nhật có ";

                if (check.WorkingDay != 0)
                {
                    var workingDay = (check.WorkingDay ?? 0) + 1;
                    workingNameDay = "Thứ " + workingDay + " có ";
                }

                context.AddFailure(string.Format(Message.shift_overlap, check.WorkingDay == null ? "" : workingNameDay, overlapShift.FirstShift.Name, overlapShift.SecondShift.Name));
                break;
            }
        }

        private void GetShiftWorkingDayFromInGroup(IGrouping<List<byte>, TimeSheetShift> group, List<ShiftWorkingDay> groupByWorkingDays, TimeSheetShift timeSheetShift)
        {
            if (timeSheetShift == null) return;

            foreach (var key in group.Key)
            {
                var exist = groupByWorkingDays.FirstOrDefault(x => x.WorkingDay == key);
                if (exist != null)
                {
                    exist.ShiftIds.AddRange(timeSheetShift.ShiftIdsToList);
                    exist.ShiftIds = exist.ShiftIds.Distinct().ToList();
                    continue;
                }

                var listShiftIds = timeSheetShift.ShiftIdsToList;
                groupByWorkingDays.Add(new ShiftWorkingDay
                {
                    WorkingDay = key,
                    ShiftIds = listShiftIds
                });
            }
        }

        private class ShiftWorkingDay
        {
            public int? WorkingDay { get; set; }
            public List<long> ShiftIds { get; set; }
        }

        private class PairShiftOverLap
        {
            public Shift FirstShift { get; set; }
            public Shift SecondShift { get; set; }
        }
        #endregion
    }
}
