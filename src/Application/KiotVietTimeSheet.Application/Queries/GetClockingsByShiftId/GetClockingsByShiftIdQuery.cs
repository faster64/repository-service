using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsByShiftId
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingsByShiftIdQuery : QueryBase<List<ClockingDto>>
    {
        public long ShiftId { get; set; }

        public GetClockingsByShiftIdQuery(long shiftId)
        {
            ShiftId = shiftId;
        }
    }
}
