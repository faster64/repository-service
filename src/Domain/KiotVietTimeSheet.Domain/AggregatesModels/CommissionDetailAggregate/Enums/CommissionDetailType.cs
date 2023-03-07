using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums
{
    public enum CommissionDetailType
    {
        [Description("Hàng hóa")]
        Product = 1,
        [Description("Nhóm hàng hóa")]
        Category = 2,
    }
}
