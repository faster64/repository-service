using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayRateFormDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte SalaryPeriod { get; set; }
        public int Status { get; set; }
        public int TotalEmployee { get; set; }
        public MainSalaryRuleValue MainSalaryRuleValue { get; set; }
        public CommissionSalaryRuleValueV2 CommissionSalaryRuleValue { get; set; }
        public OvertimeSalaryRuleValue OvertimeSalaryRuleValue { get; set; }
        public AllowanceRuleValue AllowanceRuleValue { get; set; }
        public DeductionRuleValue DeductionRuleValue { get; set; }
    }
}