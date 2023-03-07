namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionRuleValueDetail
    {
        public long DeductionId { get; set; }
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public int Rank { get; set; }
        public DeductionTypes Type { get; set; }

        public int DeductionRuleId { get; set; }
        public int DeductionTypeId { get; set; }
        public int BlockTypeTimeValue { get; set; }
        public int BlockTypeMinuteValue { get; set; }
        public bool IsEqual(DeductionRuleValueDetail detail) => detail != null && DeductionId == detail.DeductionId && Value == detail.Value && Utilities.Utilities.CompareDouble(ValueRatio.GetValueOrDefault(), detail.ValueRatio.GetValueOrDefault()) && Type == detail.Type;
    }
}
