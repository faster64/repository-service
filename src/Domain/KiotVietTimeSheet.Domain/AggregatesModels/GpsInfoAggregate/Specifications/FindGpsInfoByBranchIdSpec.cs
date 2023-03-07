using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Specifications
{
    public class FindGpsInfoByBranchIdSpec : ExpressionSpecification<GpsInfo>
    {
        public FindGpsInfoByBranchIdSpec(int branchId)
           : base(g => g.BranchId == branchId) { }
    }
}
