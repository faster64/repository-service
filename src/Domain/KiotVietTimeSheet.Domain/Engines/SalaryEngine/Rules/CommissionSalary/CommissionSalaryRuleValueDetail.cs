namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommissionSalary
{
    public class CommissionSalaryRuleValueDetail
    {
        public decimal Commission { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public int Rank { get; set; }

        public bool IsEqual(CommissionSalaryRuleValueDetail detail) => detail != null && Commission == detail.Commission && Value == detail.Value && Utilities.Utilities.CompareDouble(ValueRatio.GetValueOrDefault(), detail.ValueRatio.GetValueOrDefault());

    }
}
