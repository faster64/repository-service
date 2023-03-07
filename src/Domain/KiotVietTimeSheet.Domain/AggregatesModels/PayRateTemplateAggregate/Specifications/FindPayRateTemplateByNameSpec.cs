using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Specifications
{
    public class FindPayRateTemplateByNameSpec : ExpressionSpecification<PayRateTemplate>
    {
        public FindPayRateTemplateByNameSpec(string name)
            : base(e => e.Name.Trim().ToLower() == name.Trim().ToLower())
        { }
    }
}
