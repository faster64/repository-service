using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class RejectClockingData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public List<Clocking> ListClockings { get; set; }
        public RejectClockingData(int retailerId, List<Clocking> listClockings)
        {
            RetailerId = retailerId;
            ListClockings = listClockings;
        }
    }
}
