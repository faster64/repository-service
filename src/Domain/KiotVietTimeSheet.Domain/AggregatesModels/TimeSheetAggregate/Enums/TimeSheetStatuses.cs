using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums
{
    public enum TimeSheetStatuses
    {
        [Description("Đã hủy")]
        Void = 0,
        [Description("Đã tạo")]
        Created = 1,
    }
}
