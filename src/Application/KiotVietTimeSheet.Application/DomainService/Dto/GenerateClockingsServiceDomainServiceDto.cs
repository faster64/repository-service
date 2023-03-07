using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class GenerateClockingsServiceDomainServiceDto
    {
        public List<TimeSheet> TimeSheets { get; set; }
        public List<Clocking> ClockingsOverlapTime { get; set; }
        public List<Clocking> ClockingsOverlapByDayOff { get; set; }
        public bool IsValid { get; set; }
        public IList<string> ValidationErrors { get; set; }
    }
}
