using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class CreatedConfirmClockingHistoryEvent : DomainEvent
    {
        public ConfirmClockingHistory ConfirmClockingHistory { get; set; }

        public CreatedConfirmClockingHistoryEvent(ConfirmClockingHistory confirmClockingHistory)
        {
            ConfirmClockingHistory = confirmClockingHistory;
        }
    }
}
