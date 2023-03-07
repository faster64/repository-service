using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetGpsInfoById
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Read)]
    public class GetGpsInfoByIdQuery : QueryBase<GpsInfoDto>
    {
        public long Id { get; set; }

        public GetGpsInfoByIdQuery(long id)
        {
            Id = id;
        }
    }
    
}
