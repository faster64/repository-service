using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class FindPayRateByTemplateIdSpecification : ExpressionSpecification<PayRate>
    {
        public FindPayRateByTemplateIdSpecification(long id)
            : base(e => e.PayRateTemplateId == id)
        { }
    }
}
