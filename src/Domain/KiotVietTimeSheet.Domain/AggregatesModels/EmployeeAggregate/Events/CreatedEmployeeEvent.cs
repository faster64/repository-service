using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events
{
    public class CreatedEmployeeEvent : DomainEvent
    {
        public Employee Employee { get; set; }

        public CreatedEmployeeEvent(Employee employee)
        {
            Employee = employee;
        }
    }
}
