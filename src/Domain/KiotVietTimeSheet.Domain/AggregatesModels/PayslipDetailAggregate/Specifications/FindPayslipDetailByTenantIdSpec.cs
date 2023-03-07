using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipDetailAggregate.Specifications
{
    public class FindPayslipDetailByTenantIdSpec: ExpressionSpecification<PayslipDetail>
    {
        public FindPayslipDetailByTenantIdSpec(int tenantId, string ruleType)
            : base(p => p.TenantId == tenantId && p.RuleType == ruleType)
        { }
    }
}
