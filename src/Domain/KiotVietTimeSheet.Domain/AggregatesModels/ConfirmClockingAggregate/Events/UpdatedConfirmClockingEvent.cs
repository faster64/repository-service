using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events
{
    public class UpdatedConfirmClockingEvent : DomainEvent
    {
        public ConfirmClocking ConfirmClocking { get; set; }

        public UpdatedConfirmClockingEvent(ConfirmClocking confirmClocking)
        {
            ConfirmClocking = confirmClocking;
        }
    }
}
