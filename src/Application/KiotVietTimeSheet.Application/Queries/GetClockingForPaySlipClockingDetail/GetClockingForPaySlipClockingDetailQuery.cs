using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetClockingForPaySlipClockingDetail
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingForPaySlipClockingDetailQuery : QueryBase<PayslipClockingDetailPagingDataSource>
    {
        public ISqlExpression Query { get; set; }

        public GetClockingForPaySlipClockingDetailQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
