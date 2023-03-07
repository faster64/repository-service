using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Enum
{
    public enum CommissionSettingType
    {
        [Description("Nhận toàn bộ doanh thu dịch vụ")]
        GetAll = 0,
        [Description("Chia đều doanh thu dịch vụ")]
        DivideEqually = 1
    }
}
