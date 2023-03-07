using System.ComponentModel;

namespace KiotVietTimeSheet.Domain.Utilities.Enums
{
    public enum ErrorCode
    {
        [Description("Vượt mức tối đa  nhân viên với gói cơ bản")]
        RunOutOfQuotaBasicContract,
        [Description("Quá thời hạn sử dụng")]
        ExpiriedDate,
        [Description("Vươt quá giới hạn nhân viên của cửa hàng")]
        RunOutOfQuotaBlockEmployee,
    }
}
