using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class BranchSpecification : ExpressionSpecification<IBranchId>
    {
        public BranchSpecification(int branchId, bool defaultIfEmpty = true)
          : base(entity => entity is IBranchId ? entity.BranchId == branchId : defaultIfEmpty)
        {

        }
    }
}
