using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Events
{
    public class DeletedDepartmentEvent : DomainEvent
    {
        public Department Department { get; set; }

        public DeletedDepartmentEvent(Department department)
        {
            Department = department;
        }
    }
}
