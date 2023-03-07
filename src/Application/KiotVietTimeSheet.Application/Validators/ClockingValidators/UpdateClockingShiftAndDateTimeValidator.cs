using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using System;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.ClockingValidators
{
    public class UpdateClockingShiftAndDateTimeValidator : UpdateClockingValidator
    {
        #region PROPERTIES

        #endregion

        #region CONSTRUCTOR

        public UpdateClockingShiftAndDateTimeValidator(
            IClockingReadOnlyRepository clockingRepository,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            long oldShiftId,
            long targetShiftId,
            DateTime startTime,
            DateTime endTime
        ) : base(clockingRepository, shiftReadOnlyRepository, oldShiftId, targetShiftId, startTime, endTime)
        {
            ValidateClockingStatus();
        }

        #endregion

        #region Protected methods
        protected void ValidateClockingStatus()
        {
            RuleFor(clocking => clocking.ClockingStatus)
                .Must(clockingStatus => clockingStatus == (byte)ClockingStatuses.Created)
                .WithMessage(Message.clocking_cannotChangeShiftForClockingTimekeepingOrDayOff);
        }
        #endregion
    }
}
