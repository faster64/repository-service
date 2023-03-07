using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipById
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPayslipByIdQuery : QueryBase<PayslipDto>
    {
        public long Id { get; set; }

        public GetPayslipByIdQuery(long id)
        {
            Id = id;
        }
    }
}
