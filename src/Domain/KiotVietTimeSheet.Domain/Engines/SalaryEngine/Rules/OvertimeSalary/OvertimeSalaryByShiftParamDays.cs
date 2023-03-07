using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryByShiftParamDays
    {
        public decimal Value { get; set; }
        public decimal CalculatedValue { get; set; }
        public SalaryDays Type { get; set; }
    }
}
