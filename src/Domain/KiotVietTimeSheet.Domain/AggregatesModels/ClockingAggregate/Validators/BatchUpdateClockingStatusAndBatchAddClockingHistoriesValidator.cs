using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators
{
    public class BatchUpdateClockingStatusAndBatchAddClockingHistoriesValidator : BaseClockingValidator<Clocking>
    {
        public BatchUpdateClockingStatusAndBatchAddClockingHistoriesValidator(List<Clocking> clockings)
        {
            ValidateSameWorkingTime(clockings);
            ValidateMaximumSelectedClocking(clockings);
        }


    }
}
