using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class CreatedConfirmClockingEvent : DomainEvent
    {
        public ConfirmClocking ConfirmClocking { get; set; }

        public CreatedConfirmClockingEvent(ConfirmClocking confirmClocking)
        {
            ConfirmClocking = confirmClocking;
        }
    }
}
