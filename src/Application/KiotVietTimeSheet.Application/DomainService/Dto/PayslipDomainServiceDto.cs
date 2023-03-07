using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class PayslipDomainServiceDto
    {
        public bool IsValid { get; set; }
        public IList<string> ValidationErrors { get; set; }
    }
}
