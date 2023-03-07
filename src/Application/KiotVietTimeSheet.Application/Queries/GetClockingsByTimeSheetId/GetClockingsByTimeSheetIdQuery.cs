using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsByTimeSheetId
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingsByTimeSheetIdQuery : QueryBase<List<ClockingDto>>
    {
        public long TimeSheetId { get; set; }

        public GetClockingsByTimeSheetIdQuery(long timeSheetId)
        {
            TimeSheetId = timeSheetId;
        }
    }
}
