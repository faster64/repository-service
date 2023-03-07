using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
namespace KiotVietTimeSheet.Application.Commands.SwapClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class SwapClockingCommand : BaseCommand<SwapClockingResultDto>
    {
        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public long SourceEmployeeId { get; set; }
        public long TargetEmployeeId { get; set; }

        public SwapClockingCommand(long sourceId, long targetId, long sourceEmployeeId, long targetEmployeeId)
        {
            SourceId = sourceId;
            TargetId = targetId;
            SourceEmployeeId = sourceEmployeeId;
            TargetEmployeeId = targetEmployeeId;
        }
    }
}
