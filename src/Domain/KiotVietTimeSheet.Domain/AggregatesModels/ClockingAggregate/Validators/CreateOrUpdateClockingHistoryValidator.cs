using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators
{
    public class CreateOrUpdateClockingHistoryValidator : ClockingHistoryValidator<ClockingHistory>
    {
        public CreateOrUpdateClockingHistoryValidator(DateTime clockingStartTime, DateTime clockingEndTime)
        {
            ValidateCheckedInDate(clockingEndTime);
            ValidateCheckedOutDate(clockingStartTime);
        }
    }
}
