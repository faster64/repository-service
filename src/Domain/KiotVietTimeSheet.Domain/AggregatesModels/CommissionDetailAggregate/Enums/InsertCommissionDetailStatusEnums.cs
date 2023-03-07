using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums
{
    public enum InsertCommissionDetailStatusEnums
    {
        [Description("Đang thực hiện")]
        InProgress = 1,
        [Description("Hoàn thành")]
        Completed = 2,
        [Description("Lỗi")]
        Error = 3
    }
}
