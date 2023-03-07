using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;

namespace KiotVietTimeSheet.Application.Dto
{
    public class DeductionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int TenantId { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
        public bool? SelectedItem { get; set; }
        public int? ValueType { get; set; }

        #region DeductionRuleValueDetail

        public decimal? Value { get; set; }
        public double? ValueRatio { get; set; }

        // trễ - sớm
        public DeductionTypes DeductionRuleId { get; set; }

        // phút - lần
        public DeductionRuleTypes DeductionTypeId { get; set; }

        public int BlockTypeTimeValue { get; set; }
        public int BlockTypeMinuteValue { get; set; }

        #endregion DeductionRuleValueDetail
    }
}