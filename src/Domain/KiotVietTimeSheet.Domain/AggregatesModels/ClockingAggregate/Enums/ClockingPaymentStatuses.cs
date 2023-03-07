using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums
{
    public enum ClockingPaymentStatuses
    {
        [Description("Chưa chốt lương")]
        UnPaid = 0,
        [Description("Đã chốt lương")]
        Paid = 1
    }
}
