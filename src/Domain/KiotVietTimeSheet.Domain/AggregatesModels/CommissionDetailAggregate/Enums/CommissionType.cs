using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums
{
    public enum CommissionType
    {
        [Description("Hoa hồng thực hiện dịch vụ")]
        Service = 1,
        [Description("Hoa hồng tư vấn")]
        Counselor = 2
    }
    public enum CommissionSetting
    {
        [Description("Nhận toàn bộ")]
        GetAll = 0,
        [Description("Chia đều")]
        EquallyShare = 1
    }
    public enum ObjectType
    {
        [Description("Sản phẩm")]
        Product = 1,
        [Description("Nhóm sản phẩm")]
        Category = 2
    }
}
