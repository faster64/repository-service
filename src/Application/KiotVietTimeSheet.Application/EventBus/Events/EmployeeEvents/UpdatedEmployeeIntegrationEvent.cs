using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents
{
    public class UpdatedEmployeeIntegrationEvent : IntegrationEvent
    {
        public Employee OldEmployee { get; set; }
        public Employee NewEmployee { get; set; }
        public PayRate PayRate { get; set; }
        public List<int> WorkBranchIds { get; set; }

        public UpdatedEmployeeIntegrationEvent(Employee oldEmployee, Employee newEmployee, PayRate payRate, List<int> workBranchIds)
        {
            OldEmployee = oldEmployee;
            NewEmployee = newEmployee;
            PayRate = payRate;
            WorkBranchIds = workBranchIds;
        }

        [JsonConstructor]
        public UpdatedEmployeeIntegrationEvent()
        {

        }
    }
}
