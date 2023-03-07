using System.Collections.Generic;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionSalaryRuleParamV2 : IRuleParam
    {
        public CommissionSalaryTypes Type { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCounselorRevenue { get; set; }
        public decimal TotalGrossProfit { get; set; }
        public decimal? CommissionSalary { get; set; }
        public decimal CommissionSalaryOrigin { get; set; }
        public List<CommissionParam> CommissionParams { get; set; }
    }
}
