using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class CreateTimeSheetClockingWhenReplaceEmployeeValidator : BaseTimeSheetValidator<List<TimeSheet>>
    {
        #region Properties
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        #endregion

        public CreateTimeSheetClockingWhenReplaceEmployeeValidator(
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            List<ClockingDto> clockingsOverlapTime,
                List<Clocking> timeSheetClockings
            )
        {
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;

            ValidateEmployee();
            ValidateShift();
            ValidateClockingsOverlap(clockingsOverlapTime, timeSheetClockings);
        }

        #region Protected methods
        protected void ValidateClockingsOverlap(List<ClockingDto> listClockingOverlapTime, List<Clocking> timeSheetListClocking)
        {
            RuleFor(x => x)
               .Custom((timeSheets, context) =>
               {
                   if (timeSheets == null || !timeSheets.Any()) return;
                   var timeSheet = timeSheets.First();

                   if (timeSheet.Id != 0) return;

                   var activeListClocking = timeSheetListClocking?.Where(x => x.ClockingStatus != (byte)ClockingStatuses.Void && !x.IsDeleted);

                   if (activeListClocking != null && activeListClocking.Any()) return;
                   
                   if (listClockingOverlapTime.Any(x => x.TimeSheetId == timeSheet.Id))
                   {
                       context.AddFailure(Message.clocking_replaceEmployeeOverlapTime);
                   }
                   
               });
        }

        protected void ValidateEmployee()
        {
            RuleFor(timeSheets => timeSheets)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .CustomAsync(async (timeSheets, context, token) =>
                {
                    var employee = await _employeeReadOnlyRepository.FindBySpecificationAsync(new FindEmployeeByIdSpec(timeSheets.Select(x => x.EmployeeId).FirstOrDefault()), false, true);
                    if (employee == null)
                    {
                        context.AddFailure(Message.timeSheet_replacementEmployeeDoesNotExist);
                        return;
                    }

                    if (employee.IsDeleted)
                    {
                        context.AddFailure(Message.timeSheet_replacementEmployeeDoesNotExist);
                    }

                    if (timeSheets.Any(timeSheet => timeSheet.BranchId != employee.BranchId))
                    {
                        context.AddFailure(Message.timeSheet_differenceBetweenReplacementEmployeeBranchIdAndTSBranchId);
                    }
                });
        }

        protected void ValidateShift()
        {
            RuleFor(timeSheets => timeSheets)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .CustomAsync(async (timeSheets, context, token) =>
                {
                    var shifts = await _shiftReadOnlyRepository.GetBySpecificationAsync(
                        new FindShiftByShiftIdsSpec(timeSheets.SelectMany(x => x.TimeSheetShifts).SelectMany(x => x.ShiftIdsToList).Distinct().ToList()), false, true);
                    
                    if (!shifts.Any())
                    {
                        context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist,
                            string.Join(", ", Label.shift)));
                        return;
                    }
                    
                    var deletedShifts = shifts.Where(s => s.IsDeleted).ToList();
                    var inactiveShifts = shifts.Where(s => !s.IsActive).ToList();

                    if (deletedShifts.Any())
                        context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist,
                            string.Join(", ", deletedShifts.Select(s => s.Name))));
                    
                    if (inactiveShifts.Any())
                        context.AddFailure(string.Format(Message.timeSheet_objectsInactive,
                            string.Join(", ", inactiveShifts.Select(s => s.Name))));

                    var isTimeSheetExist = timeSheets.Any(x => shifts.Any(s => x.BranchId != s.BranchId));
                    if (isTimeSheetExist)
                        context.AddFailure(Message.timeSheet_differenceBetweenShiftBranchIdAndTSBranchId);
                });
        }
        #endregion
    }
}
