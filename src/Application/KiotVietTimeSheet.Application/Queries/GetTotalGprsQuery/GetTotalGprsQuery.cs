using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetTotalGprsQuery
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Read)]
    public class GetTotalGprsQuery : QueryBase<long>
    {

    }
}