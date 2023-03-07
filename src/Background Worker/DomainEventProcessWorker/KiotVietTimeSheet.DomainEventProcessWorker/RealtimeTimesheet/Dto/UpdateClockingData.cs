using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class UpdateClockingData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public List<Clocking> ListClockings { get; set; }
        public UpdateClockingData(int retailerId, List<Clocking> listClockings)
        {
            RetailerId = retailerId;
            ListClockings = listClockings;
        }
    }
}
