using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class DeletedClockingEvent : DomainEvent
    {
        public Clocking Clocking { get; private set; }
        public DeletedClockingEvent(Clocking clocking)
        {
            Clocking = clocking;
        }
    }
}
