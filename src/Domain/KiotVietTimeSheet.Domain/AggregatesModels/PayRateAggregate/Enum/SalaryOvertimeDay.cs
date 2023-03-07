using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum
{
    public enum SalaryDays
    {
        [Display(Name = "sunday")]
        [Description("Chủ nhật")]
        Sunday = 0,

        [Display(Name = "saturday")]
        [Description("Thứ 7")]
        Saturday = 6,

        [Display(Name = "default")]
        [Description("Ngày thường")]
        Default = 7,

        [Display(Name = "dayOff")]
        [Description("Ngày nghỉ")]
        DayOff = 8,

        [Display(Name = "holiday")]
        [Description("Ngày lễ tết")]
        Holiday = 9,
    }
}
