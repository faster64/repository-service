using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class TimeSheetDomainServiceDto
    {
        public List<TimeSheetDto> TimeSheets { get; set; }
        public bool IsValid { get; set; }
        public IList<string> ValidationErrors { get; set; }
    }
}
