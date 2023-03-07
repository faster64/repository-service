using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class DeletedConfirmClockingHistoryEvent : DomainEvent
    {
        public ConfirmClockingHistory ConfirmClockingHistory { get; set; }

        public DeletedConfirmClockingHistoryEvent(ConfirmClockingHistory confirmClockingHistory)
        {
            ConfirmClockingHistory = confirmClockingHistory;
        }
    }
}
