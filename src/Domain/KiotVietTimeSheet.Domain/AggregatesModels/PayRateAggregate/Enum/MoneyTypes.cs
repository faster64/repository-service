using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum
{
    public enum MoneyTypes
    {
        [Display(Name = "VND")]
        [Description("Tính theo VNĐ")]
        Money = 1,
        [Display(Name = "%")]
        [Description("Tính theo %")]
        Percent = 2,
    }
}