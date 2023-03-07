namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceParam
    {
        public long AllowanceId { get; set; }
        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }
        public string Name { get; set; }
        public decimal? CalculatedValue { get; set; }
        public double? CalculatedValueRatio { get; set; }
        public int NumberWorkingDay { get; set; }
        public bool? SelectedItem { get; set; }
        public AllowanceTypes Type { get; set; }
        public int StandardWorkingDayNumber { get; set; }
        public bool? IsChecked { get; set; }
    }
}
