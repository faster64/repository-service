using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;
using System.Linq;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByExistedSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByExistedSpec(int tennantId, ICollection<long> productIds, ICollection<long> commissionIds) :
            base(x => !x.IsDeleted &&
                      x.TenantId == tennantId &&
                      productIds.Contains(x.ObjectId) &&
                      commissionIds.Contains(x.CommissionId) &&
                      x.Type == (byte)CommissionDetailType.Product)
        {
        }

        public FindCommissionDetailByExistedSpec(int tennantId, ICollection<CommissionDetail> ls) :
            this(tennantId,
                ls.Select(x => x.ObjectId).ToList(),
                ls.Select(x => x.CommissionId).ToList())
        {
        }
    }
}