using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public enum DeductionTypes
    {
        [Description("Giảm trừ cố định")]
        Fixed = 0,
        [Description("Giảm trừ đi muộn")]
        Late = 1, 
        [Description("Giảm trừ về sớm")]
        Early = 2
    }

    public enum DeductionRuleTypes
    {
        [Description("Tính theo block phút")]
        Minute = 1,
        [Description("Tính theo lần")]
        Time = 2
    }
}
