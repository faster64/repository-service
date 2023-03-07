using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionById
{
    [RequiredPermission(TimeSheetPermission.Commission_Read)]
    public class GetCommissionByIdQuery : QueryBase<CommissionDto>
    {
        public long Id { get; set; }

        public GetCommissionByIdQuery(long id)
        {
            Id = id;
        }
    }
}
