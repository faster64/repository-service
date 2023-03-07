using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums
{
    public enum RepeatTypes
    {
        [Description("Hàng ngày")]
        Daily = 1,
        [Description("Hàng tuần")]
        Weekly = 2
    }
}
