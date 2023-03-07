using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class CreatedClockingData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public List<Clocking> ListClockings { get; set; }
        public List<Employee> ListEmployees { get; set; }
        public CreatedClockingData(int retailerId, List<Clocking> listClockings)
        {
            RetailerId = retailerId;
            ListClockings = listClockings;
            ListEmployees = ListClockings.Select(x => x.Employee).Distinct().ToList();
        }
    }
}
