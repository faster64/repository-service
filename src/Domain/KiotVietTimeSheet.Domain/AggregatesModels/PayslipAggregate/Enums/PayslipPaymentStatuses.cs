using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums
{
    public enum PayslipPaymentStatuses
    {
        [Description("Đã thanh toán")]
        Paid = 0,
        [Description("Đã hủy")]
        Void = 1,
    }
}
