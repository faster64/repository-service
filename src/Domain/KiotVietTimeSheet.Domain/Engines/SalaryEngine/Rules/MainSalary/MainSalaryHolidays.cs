
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryHolidays
    {
        public MoneyTypes MoneyTypes { get; set; }
        public SalaryDays Type { get; set; }
        public decimal Value { get; set; }
        public bool IsApply { get; set; }
        public int Sort { get; set; }
        public bool IsEqual(MainSalaryHolidays holiday) =>
            holiday != null &&
            MoneyTypes == holiday.MoneyTypes &&
            Type == holiday.Type &&
            Value == holiday.Value &&
            IsApply == holiday.IsApply;
    }
}
