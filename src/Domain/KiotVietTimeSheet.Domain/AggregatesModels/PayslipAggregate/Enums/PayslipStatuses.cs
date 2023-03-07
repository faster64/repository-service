using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums
{
    public enum PayslipStatuses
    {
        [Description("Đã hủy")]
        Void = 0,
        [Description("Tạm tính")]
        TemporarySalary = 1,
        [Description("Đã chốt lương")]
        PaidSalary = 2,
        [Description("Nháp")]
        Draft = 3
    }
}
