using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingGpsDto
    {
        public List<ClockingDto> ClockingsDto { get; set; }
        public List<ShiftDto> ShiftsDto { get; set; }
        public string BranchName { get; set; }
        public List<ConfirmClockingDto> ConfirmClockingDto { get; set; }
    }
}
