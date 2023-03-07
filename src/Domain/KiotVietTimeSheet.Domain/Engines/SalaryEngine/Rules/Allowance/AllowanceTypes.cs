using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public enum AllowanceTypes
    {
        [Description("Phụ cấp cố định")]
        FixedAllowance = 0,
        [Description("Phụ cấp cho mỗi ngày công làm việc")]
        BaseOnWorkingDay = 1,
        [Description("Phụ cấp trên ngày công chuẩn")]
        BaseOnDayStandard = 2
    }
}
