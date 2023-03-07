using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetListShift
{
    public sealed class GetListShiftQuery : QueryBase<PagingDataSource<ShiftDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetListShiftQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
