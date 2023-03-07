
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryDays
    {
        public MoneyTypes MoneyTypes { get; set; }
        public SalaryDays Type { get; set; }
        public decimal Value { get; set; }
        public bool IsApply { get; set; }
        public int Sort { get; set; }
    }
}
