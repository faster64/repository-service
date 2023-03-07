using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Events
{
    public class UpdatedDepartmentEvent : DomainEvent
    {
        public Department Department { get; set; }

        public UpdatedDepartmentEvent(Department department)
        {
            Department = department;
        }
    }
}
