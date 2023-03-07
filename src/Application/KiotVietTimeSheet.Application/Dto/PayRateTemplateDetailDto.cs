namespace KiotVietTimeSheet.Application.Dto
{
    public class PayRateTemplateDetailDto
    {
        public long Id { get; set; }
        public long PayRateTemplateId { get; set; }
        public string RuleType { get; set; }
        public string RuleValue { get; set; }
    }
}
