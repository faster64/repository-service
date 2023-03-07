using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class FindPayRateByEmployeeIdsSpec : ExpressionSpecification<PayRate>
    {
        public FindPayRateByEmployeeIdsSpec(IReadOnlyCollection<long> employeeIds)
            : base(e => employeeIds.Contains(e.EmployeeId))
        { }
    }
}
