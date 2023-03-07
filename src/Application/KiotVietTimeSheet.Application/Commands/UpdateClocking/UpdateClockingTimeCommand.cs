using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class UpdateClockingTimeCommand : BaseCommand<ClockingDto>
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long ShiftId { get; set; }
        public string ShiftName { get; set; }
        public long From { get; set; }
        public long To { get; set; }

        public UpdateClockingTimeCommand(int tenantId, int branchId, long shiftId, string shiftName, long from, long to)
        {
            TenantId = tenantId;
            BranchId = branchId;
            ShiftId = shiftId;
            ShiftName = shiftName;
            From = from;
            To = to;
        }
    }
}