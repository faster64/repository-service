using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipClockingDetailPagingDataSource : PagingDataSource<ClockingDto>
    {
        public int TotalTimeIsLate { get; set; }
        public int TotalOverTimeBeforeShiftWork { get; set; }
        public int TotalTimeIsLeaveWorkEarly { get; set; }
        public int TotalOverTimeAfterShiftWork { get; set; }
    }
}
