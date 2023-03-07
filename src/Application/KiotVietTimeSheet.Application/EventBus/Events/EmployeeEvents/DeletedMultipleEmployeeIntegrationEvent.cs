using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents
{
    public class DeletedMultipleEmployeeIntegrationEvent : IntegrationEvent
    {
        public List<Employee> ListEmployees { get; set; }

        public DeletedMultipleEmployeeIntegrationEvent(List<Employee> listEmployees)
        {
            ListEmployees = listEmployees;
        }

        [JsonConstructor]
        public DeletedMultipleEmployeeIntegrationEvent()
        {

        }
    }
}
