using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events
{
    public class UpdatedEmployeeEvent : DomainEvent
    {
        public Employee OldEmployee { get; set; }
        public Employee NewEmployee { get; set; }
        public UpdatedEmployeeEvent(Employee oldEmployee, Employee newEmployee)
        {
            OldEmployee = oldEmployee;
            NewEmployee = newEmployee;
        }
    }
}
