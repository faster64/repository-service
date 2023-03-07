using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class EmployeeData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public Employee Employee { get; set; }
        public EmployeeData(int retailerId, Employee employee)
        {
            RetailerId = retailerId;
            Employee = employee;
        }
    }
}
