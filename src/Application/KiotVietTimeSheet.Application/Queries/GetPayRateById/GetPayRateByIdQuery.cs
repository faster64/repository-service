using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateById
{
    [RequiredPermission(TimeSheetPermission.PayRate_Read)]
    public class GetPayRateByIdQuery : QueryBase<PayRateDto>
    {
        public long Id { get; set; }

        public GetPayRateByIdQuery(long id)
        {
            Id = id;
        }
    }
}
