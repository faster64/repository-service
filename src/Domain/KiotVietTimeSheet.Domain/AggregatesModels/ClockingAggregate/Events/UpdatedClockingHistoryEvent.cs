using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class UpdatedClockingHistoryEvent : DomainEvent
    {
        public ClockingHistory OldClockingHistory { get; set; }
        public ClockingHistory NewClockingHistory { get; set; }

        public UpdatedClockingHistoryEvent(ClockingHistory oldClockingHistory, ClockingHistory newClockingHistory)
        {
            OldClockingHistory = oldClockingHistory;
            NewClockingHistory = newClockingHistory;
        }
    }
}
