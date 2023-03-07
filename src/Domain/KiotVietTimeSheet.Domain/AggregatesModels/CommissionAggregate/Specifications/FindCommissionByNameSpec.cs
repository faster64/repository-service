using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByNamesSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByNamesSpec(List<string> names)
            : base(s => names.Contains(s.Name)) { }
    }
}
