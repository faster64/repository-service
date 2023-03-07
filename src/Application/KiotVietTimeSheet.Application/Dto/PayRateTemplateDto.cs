using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayRateTemplateDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte SalaryPeriod { get; set; }
        public int Status { get; set; }
        public List<PayRateTemplateDetailDto> PayRateTemplateDetails { get; set; }
    }
}
