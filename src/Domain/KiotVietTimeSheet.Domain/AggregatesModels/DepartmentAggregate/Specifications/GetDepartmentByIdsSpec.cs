using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Specifications
{
    public class GetDepartmentByIdsSpec : ExpressionSpecification<Department>
    {
        public GetDepartmentByIdsSpec(ICollection<long?> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
