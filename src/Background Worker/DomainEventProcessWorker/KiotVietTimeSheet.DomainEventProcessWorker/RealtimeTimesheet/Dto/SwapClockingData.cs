using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class SwapClockingData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public Clocking SourceClockings { get; set; }
        public Clocking TargetClockings { get; set; }
        public SwapClockingData(int retailerId, Clocking sourceClockings, Clocking targetClockings)
        {
            RetailerId = retailerId;
            SourceClockings = sourceClockings;
            TargetClockings = targetClockings;
        }
    }
}
