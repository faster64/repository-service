using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class LimitByBranchAccessSpec<TEntity> : ExpressionSpecification<TEntity> where TEntity : class
    {
        public LimitByBranchAccessSpec(List<int> branchIds)
           : base(entity => branchIds.Contains(((IBranchId)entity).BranchId))
        {

        }
    }
}
