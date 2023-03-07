using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetByLongIdsSpec : ExpressionSpecification<Employee>
    {
        public GetByLongIdsSpec(List<long> ids)
          : base(entity => ids.Contains(entity.Id))
        {

        }
    }
}
