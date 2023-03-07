namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceRuleValueDetail
    {
        public long AllowanceId { get; set; }
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public int Rank { get; set; }
        public AllowanceTypes Type { get; set; }
        public bool? IsChecked { get; set; }
        public bool IsEqual(AllowanceRuleValueDetail detail)
        {
            return detail != null && AllowanceId == detail.AllowanceId && Value == detail.Value && Utilities.Utilities.CompareDouble(ValueRatio.GetValueOrDefault(), detail.ValueRatio.GetValueOrDefault()) && Type == detail.Type && IsChecked == detail.IsChecked;
        }

    }
}
