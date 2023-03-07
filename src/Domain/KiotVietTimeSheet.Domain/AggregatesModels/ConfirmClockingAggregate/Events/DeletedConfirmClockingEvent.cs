using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class DeletedConfirmClockingEvent : DomainEvent
    {
        public ConfirmClocking ConfirmClocking { get; set; }

        public DeletedConfirmClockingEvent(ConfirmClocking confirmClocking)
        {
            ConfirmClocking = confirmClocking;
        }
    }
}
