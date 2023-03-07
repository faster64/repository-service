using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetAllowanceById
{
    [RequiredPermission(TimeSheetPermission.Allowance_Read)]
    public class GetAllowanceByIdQuery : QueryBase<AllowanceDto>
    {
        public long Id { get; set; }

        public GetAllowanceByIdQuery(long id)
        {
            Id = id;
        }
    }
}
