using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents
{
    public class DeletedEmployeeIntegrationEvent : IntegrationEvent
    {
        public Employee Employee { get; set; }

        public DeletedEmployeeIntegrationEvent(DeletedEmployeeEvent @event)
        {
            Employee = @event.Employee;
        }

        [JsonConstructor]
        public DeletedEmployeeIntegrationEvent()
        {

        }
    }
}
