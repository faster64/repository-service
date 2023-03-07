namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipDetailDto
    {
        public long Id { get; set; }
        public long PayslipId { get; set; }
        public string RuleType { get; set; }
        public string RuleValue { get; set; }
        public string RuleParam { get; set; }
    }
}
