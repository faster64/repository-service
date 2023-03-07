using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class CreatedClockingHistoryEvent : DomainEvent
    {
        public ClockingHistory ClockingHistory { get; set; }

        public CreatedClockingHistoryEvent(ClockingHistory clockingHistory)
        {
            ClockingHistory = clockingHistory;
        }
    }
}
