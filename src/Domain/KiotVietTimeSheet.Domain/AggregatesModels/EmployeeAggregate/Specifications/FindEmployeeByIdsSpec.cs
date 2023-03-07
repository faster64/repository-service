using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByIdsSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByIdsSpec(ICollection<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
