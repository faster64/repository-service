using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class RejectedClockingEvent : DomainEvent
    {
        public Clocking Clocking { get; set; }
        public RejectedClockingEvent(Clocking clocking)
        {
            Clocking = clocking;
        }
    }
}
