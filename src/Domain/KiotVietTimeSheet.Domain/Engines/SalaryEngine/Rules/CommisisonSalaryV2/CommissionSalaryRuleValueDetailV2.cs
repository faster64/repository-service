namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionSalaryRuleValueDetailV2
    {
        public string Group { get; set; }
        public byte CommissionType { get; set; }
        public decimal? CommissionLevel { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public long? CommissionTableId { get; set; }

        public bool IsEqual(CommissionSalaryRuleValueDetailV2 detail)
        {
            if (detail == null) return false;
            if (CommissionLevel.GetValueOrDefault() != detail.CommissionLevel.GetValueOrDefault()) return false;
            if (CommissionTableId.GetValueOrDefault() != 0 && detail.CommissionTableId.GetValueOrDefault() != 0)
                return CommissionTableId.GetValueOrDefault() == detail.CommissionTableId.GetValueOrDefault();
            return Value == detail.Value && ValueRatio == detail.ValueRatio;
        }
    }
}
