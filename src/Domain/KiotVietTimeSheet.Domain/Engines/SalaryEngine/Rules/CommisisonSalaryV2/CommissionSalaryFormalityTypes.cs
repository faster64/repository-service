using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public enum CommissionSalaryFormalityTypes
    {
        [Description("Theo doanh thu cá nhân")]
        PersonalCommissionRevenue = 0,
        [Description("Theo doanh thu chi nhánh")]
        BranchCommissionRevenue = 1,
        [Description("Theo lợi nhuận gộp cá nhân")]
        PersonalGrossProfit = 2
    }
}