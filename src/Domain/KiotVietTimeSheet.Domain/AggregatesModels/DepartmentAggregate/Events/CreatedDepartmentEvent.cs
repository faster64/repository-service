using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Events
{
    public class CreatedDepartmentEvent : DomainEvent
    {
        public Department Department { get; set; }

        public CreatedDepartmentEvent(Department department)
        {
            Department = department;
        }
    }
}
