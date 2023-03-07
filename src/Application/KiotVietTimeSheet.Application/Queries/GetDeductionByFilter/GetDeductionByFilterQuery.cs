using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionByFilter
{
    [RequiredPermission(TimeSheetPermission.Deduction_Read)]
    public class GetDeductionByFilterQuery : QueryBase<object>
    {
        public long DeductionIdDeleted { get; set; }
        public ISqlExpression Query { get; set; }

        public GetDeductionByFilterQuery(long deductionIdDeleted, ISqlExpression query)
        {
            DeductionIdDeleted = deductionIdDeleted;
            Query = query;
        }
    }
}
