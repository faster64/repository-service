using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class UpdatedConfirmClockingHistoryEvent : DomainEvent
    {
        public ConfirmClockingHistory ConfirmClockingHistory { get; set; }

        public UpdatedConfirmClockingHistoryEvent(ConfirmClockingHistory confirmClockingHistory)
        {
            ConfirmClockingHistory = confirmClockingHistory;
        }
    }
}
