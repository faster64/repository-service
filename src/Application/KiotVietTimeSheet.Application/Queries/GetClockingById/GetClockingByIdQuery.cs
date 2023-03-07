using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetClockingById
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingByIdQuery : QueryBase<ClockingDto>
    {
        public long Id { get; set; }

        public GetClockingByIdQuery(long id)
        {
            Id = id;
        }
    }
}
