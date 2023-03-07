using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetJobTitle
{
    [RequiredPermission(TimeSheetPermission.JobTitle_Read)]
    public class GetJobTitleQuery : QueryBase<PagingDataSource<JobTitleDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetJobTitleQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
