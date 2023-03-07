namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionParam
    {
        public long DeductionId { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public string Name { get; set; }
        public decimal? CalculatedValue { get; set; }
        public double? CalculatedValueRatio { get; set; }
        public bool? SelectedItem { get; set; }
        public DeductionTypes Type { get; set; }
    }
}
