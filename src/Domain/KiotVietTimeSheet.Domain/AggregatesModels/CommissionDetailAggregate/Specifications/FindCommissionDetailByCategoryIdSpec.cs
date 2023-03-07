using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByCategoryIdSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByCategoryIdSpec(long categoryId)
            : base(e => e.ObjectId == categoryId && e.Type == (byte)CommissionDetailType.Category)
        {
        }
    }
}
