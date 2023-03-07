using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public enum CommissionSalaryTypes
    {
        [Description("Tính theo mức tổng doanh thu")]
        WithTotalCommission = 1,
        [Description("Tính theo nấc bậc thang tổng doanh thu")]
        WithLevelCommission = 2,
        [Description("Tính theo mức vượt doanh thu tối thiểu")]
        WithMinimumCommission = 3,
        [Description("Tính theo mức tổng lợi nhuận gộp")]
        WithTotalPersonalGrossProfit = 4
    }
}
