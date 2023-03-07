using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using System.Collections.Generic;
using System.Linq;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers
{
    public static class AllowanceRuleHelpers
    {
        public static void AddPayRateDetailByAllowance(List<PayRateDetail> oldPayRateDetails, List<AllowanceParam> newAllowanceRule)
        {
            if (oldPayRateDetails == null) oldPayRateDetails = new List<PayRateDetail>();
            var oldAllowanceRule = oldPayRateDetails.FirstOrDefault(x => x.RuleType == typeof(AllowanceRule).Name);
            if (oldAllowanceRule == null && newAllowanceRule != null)
            {
                var allowanceRuleValue = $"{{\"{typeof(AllowanceRuleValueDetail).Name}\": []}}";
                oldPayRateDetails.Add(new PayRateDetail(typeof(AllowanceRule).Name, allowanceRuleValue));
            }
        }
    }
}
