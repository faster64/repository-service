using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models
{
    public class EmployeeBranch : BaseEntity, IEntityIdlong, ITenantId
    {

        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; protected set; }
        // [References(typeof(Employee))]
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public EmployeeBranch(int branchId, long employeeId)
        {
            BranchId = branchId;
            EmployeeId = employeeId;
        }
    }

}
