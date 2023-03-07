using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;

namespace KiotVietTimeSheet.Application.Validators.ClockingValidators
{
    public class SwapClockingValidator : BaseClockingValidator<Clocking>
    {
        #region Properties

        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly Employee _sourceEmployee;
        private readonly Employee _targetEmployee;
        #endregion

        #region Constructors

        public SwapClockingValidator(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            Clocking target,
            Employee sourceEmployee,
            Employee targetEmployee
        )
        {
            _sourceEmployee = sourceEmployee;
            _targetEmployee = targetEmployee;
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            ValidateEmployee(sourceEmployee, targetEmployee).DependentRules(() =>
            {
                ValidateShift(target).DependentRules(() => { ValidateSwapClocking(target); });
            });


        }

        #endregion

        #region Methods

        // The first one
        protected IRuleBuilderOptions<Clocking, Clocking> ValidateSwapClocking(Clocking target)
        {
            return RuleFor(x => x)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(clocking => clocking != null && target != null)
                .WithMessage(Message.clocking_notExist)
                .Custom((clocking, context) => { AddValid(target, clocking, context); })
                .MustAsync(async (clocking, token) =>
                {
                    var targetClockings = await _clockingReadOnlyRepository.GetBySpecificationAsync(
                        new FindClockingByEmployeeIdSpec(target.EmployeeId)
                            .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(target.StartTime.Date))
                            .And(new FindClockingByStartTimeLessThanOrEqualSpec(target.EndTime.Date.AddDays(1)))
                            .Not(new FindClockingByClockingIdSpec(target.Id))
                            .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)));

                    var existClockingOverlapTargetClockings = targetClockings.Any(c => IsOverLapTimeBetweenClockings(target, c));

                    var sourceClockings = await _clockingReadOnlyRepository.GetBySpecificationAsync(
                        new FindClockingByEmployeeIdSpec(clocking.EmployeeId)
                            .And(new FindClockingByStartTimeGreaterThanOrEqualSpec(clocking.StartTime.Date))
                            .And(new FindClockingByStartTimeLessThanOrEqualSpec(clocking.EndTime.Date.AddDays(1)))
                            .Not(new FindClockingByClockingIdSpec(clocking.Id))
                            .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void)));

                    var existClockingOverlapSourceClockings = sourceClockings.Any(c => IsOverLapTimeBetweenClockings(clocking, c));

                    return !existClockingOverlapTargetClockings && !existClockingOverlapSourceClockings;
                })
                .WithMessage(Message.clocking_duplicatedTimeOfShiftChange);
        }

        private void AddValid(Clocking target, Clocking clocking, CustomContext context)
        {
            if (clocking == null)
            {
                context.AddFailure(Message.clocking_notExist);
            }

            else if (clocking.ClockingStatus == (byte)ClockingStatuses.Void)
            {
                context.AddFailure(string.Format(Message.clocking_doesNotExistEmployeeName, _sourceEmployee == null ? "" : _sourceEmployee.Name.Replace(Constant.CodeDelSuffix, "")));
            }
            else if (clocking.ClockingStatus != (byte)ClockingStatuses.Created)
            {
                context.AddFailure(Message.clocking_cannotChangeShiftForClockingTimekeepingOrDayOff);
            }

            if (target == null)
            {
                context.AddFailure(Message.clocking_notExist);
            }
            else if (target.ClockingStatus == (byte)ClockingStatuses.Void)
            {
                context.AddFailure(string.Format(Message.clocking_doesNotExistEmployeeName, _targetEmployee == null ? "" : _targetEmployee.Name.Replace(Constant.CodeDelSuffix, "")));
            }
            else if (target.ClockingStatus != (byte)ClockingStatuses.Created)
            {
                context.AddFailure(Message.clocking_cannotChangeShiftForClockingTimekeepingOrDayOff);
            }
        }

        protected IRuleBuilderOptions<Clocking, Clocking> ValidateShift(Clocking target)
        {
            return (IRuleBuilderOptions<Clocking, Clocking>)RuleFor(c => c)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .CustomAsync(async (clocking, context, token) =>
                {
                    var sourceShift = await _shiftReadOnlyRepository.FindByIdAsync(clocking.ShiftId, false, true);
                    var targetShift = await _shiftReadOnlyRepository.FindByIdAsync(target.ShiftId, false, true);
                    
                    if (clocking.ShiftId == target.ShiftId && clocking.StartTime == target.StartTime && clocking.EndTime == target.EndTime)
                    {
                        context.AddFailure(Message.clocking_duplicatedTimeOfShiftChange);
                    }

                    AddFailureInShiftSwapClocking(sourceShift, context);

                    AddFailureInShiftSwapClocking(targetShift, context);
                });
        }

        private static void AddFailureInShiftSwapClocking(Shift shift, CustomContext context)
        {
            if (shift == null)
                context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, Label.shift));

            if (shift != null && shift.IsDeleted)
                context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, shift.Name));

            if (shift != null && !shift.IsActive)
                context.AddFailure(string.Format(Message.timeSheet_objectsInactive, shift.Name));
            
        }

        protected IRuleBuilderOptions<Clocking, Clocking> ValidateEmployee(Employee sourceEmployee, Employee targetEmployee)
        {
            return (IRuleBuilderOptions<Clocking, Clocking>)RuleFor(x => x)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Custom((clocking, context) =>
                {
                    if (sourceEmployee == null || sourceEmployee.IsDeleted)
                    {
                        context.AddFailure(string.Format(Message.timeSheet_employeesDoNotExist,
                            sourceEmployee == null ? "" : sourceEmployee.Name));
                    }
                    if (targetEmployee == null || targetEmployee.IsDeleted)
                    {
                        context.AddFailure(string.Format(Message.timeSheet_employeesDoNotExist,
                            targetEmployee == null ? "" : targetEmployee.Name.Replace(Constant.CodeDelSuffix, "")));
                    }
                });
        }
        #endregion
    }
}
