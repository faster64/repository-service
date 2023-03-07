using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class GenerateClockingsDomainServiceDto
    {
        public List<TimeSheet> TimeSheets { get; set; }
        public List<ClockingDto> ClockingsOverlapTime { get; set; }
        public List<Clocking> TimeSheetClockings { get; set; }
        public List<long> ClockingNeedUpdateIds { get; set; }
        public bool IsValid { get; set; }
        public IList<string> ValidationErrors { get; set; }
    }
}
