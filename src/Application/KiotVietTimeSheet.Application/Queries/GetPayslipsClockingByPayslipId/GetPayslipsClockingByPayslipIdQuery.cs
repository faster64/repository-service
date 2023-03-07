using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsClockingByPayslipId
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPayslipsClockingByPayslipIdQuery : QueryBase<PagingDataSource<PayslipClockingDto>>
    {
        public PayslipClockingByPayslipIdFilter Filter { get; set; }

        public GetPayslipsClockingByPayslipIdQuery(PayslipClockingByPayslipIdFilter filter)
        {
            Filter = filter;
        }
    }
}
