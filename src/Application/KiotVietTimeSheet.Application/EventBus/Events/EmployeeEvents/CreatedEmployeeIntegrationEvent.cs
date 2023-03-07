using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents
{
    public class CreatedEmployeeIntegrationEvent : IntegrationEvent
    {
        public Employee Employee { get; set; }
        public PayRate PayRate { get; set; }
        public List<int> WorkBranchIds { get; set; }

        public CreatedEmployeeIntegrationEvent(Employee employee, PayRate payRate, List<int> workBranchIds)
        {
            Employee = employee;
            PayRate = payRate;
            WorkBranchIds = workBranchIds;
        }

        [JsonConstructor]
        public CreatedEmployeeIntegrationEvent()
        {

        }
    }
}
