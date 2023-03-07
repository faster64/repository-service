using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using System;
using System.Linq;
using Message = KiotVietTimeSheet.Resources.Message;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Resources;
using static KiotVietTimeSheet.Domain.Utilities.Utilities;
using FluentValidation.Validators;

namespace KiotVietTimeSheet.Application.Validators.ClockingGpsValidators
{
    public class UpdateClockingGpsValidator : BaseClockingValidator<Clocking>
    {
        #region Properties
        private readonly IClockingReadOnlyRepository _clockingReadOnlyRepository;
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        #endregion

        public UpdateClockingGpsValidator(
            IClockingReadOnlyRepository clockingReadOnlyRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            long oldShiftId,
            long targetShiftId,
            DateTime startTime,
            DateTime endTime)
        {
            _clockingReadOnlyRepository = clockingReadOnlyRepository;
            _shiftReadOnlyRepository = shiftReadOnlyRepository;

            ValidateTime();
            ValidateNote();
            ValidateShift(targetShiftId, oldShiftId);
            ValidateClockingOverlapTimeAsync(startTime, endTime);
        }

        #region Protected methods
        protected void ValidateShift(long targetShiftId, long oldShiftId)
        {
            RuleFor(clocking => clocking)
                .CustomAsync(async (c, context, token) =>
                {
                    var oldShift = await _shiftReadOnlyRepository.GetShiftByIdForClockingGps(oldShiftId, false);

                    if (oldShift == null || oldShift.IsDeleted)
                    {
                        context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist,
                            oldShift == null ? Label.shift : oldShift.Name));
                    }

                    if (targetShiftId != oldShiftId)
                    {
                        var targetShift = await _shiftReadOnlyRepository.GetShiftByIdForClockingGps(targetShiftId, true);
                        AddFailureUpdateClocking(targetShift, context);
                    }
                });
        }

        private void AddFailureUpdateClocking(Shift shiftUpdate, CustomContext context)
        {
            if (shiftUpdate == null)
                context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, Label.shift));

            if (shiftUpdate != null && shiftUpdate.IsDeleted)
                context.AddFailure(string.Format(Message.timeSheet_objectsDoNotExist, shiftUpdate.Name));

            if (shiftUpdate != null && !shiftUpdate.IsActive)
                context.AddFailure(string.Format(Message.timeSheet_objectsInactive, shiftUpdate.Name));
        }

        //Chi tiết làm việc này có thời gian trùng với một chi tiết làm việc khác của cùng một nhân viên
        protected void ValidateClockingOverlapTimeAsync(DateTime startTime, DateTime endTime)
        {
            RuleFor(clocking => clocking)
               .MustAsync(async (clocking, token) =>
               {
                   var existClockingOfEmployeeInShiftTargetSpec = (new FindClockingByWorkByIdSpec(clocking.WorkById))
                                .Not(new FindClockingByClockingIdSpec(clocking.Id))
                                .And(new FindClockingOverLapTimeSpec(startTime.Date, endTime.Date.AddDays(1)))
                                .Not(new FindClockingByStatusSpec((byte)ClockingStatuses.Void));

                   var clockings = await _clockingReadOnlyRepository.GetClockingBySpecificationForClockingGpsAsync(existClockingOfEmployeeInShiftTargetSpec);
                   var existClockingOfEmployeeInShiftTarget = clockings.Any(c => IsOverLapTimeBetweenClockings(clocking, c));
                   return !existClockingOfEmployeeInShiftTarget;
               })
            .WithMessage(Message.clocking_employeeHasClockingInTime);
        }
        #endregion
    }
}
