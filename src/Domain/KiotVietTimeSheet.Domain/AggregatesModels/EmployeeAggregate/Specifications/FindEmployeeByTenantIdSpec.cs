using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByTenantIdSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByTenantIdSpec(int tenantId)
            : base(e => e.TenantId == tenantId)
        { }
    }
}
