using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommissionSalary
{
    public class CommissionSalaryRuleParam : IRuleParam
    {
        public decimal TotalRevenue { get; set; }
        public decimal CommissionSalary { get; set; }
        public decimal CalculatedCommissionSalary { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public decimal? CalculatedValue { get; set; }
        public double? CalculatedValueRatio { get; set; }
    }
}
