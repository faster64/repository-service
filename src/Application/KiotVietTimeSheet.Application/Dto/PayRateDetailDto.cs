namespace KiotVietTimeSheet.Application.Dto
{
    public class PayRateDetailDto
    {
        public long Id { get; set; }
        public long PayRateId { get; set; }
        public string RuleType { get; set; }
        public string RuleValue { get; set; }
    }
}
