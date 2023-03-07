using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeQuery : QueryBase<PagingDataSource<EmployeeDto>>
    {
        public ISqlExpression Query { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public bool IncludeFingerPrint { get; set; }
        public GetEmployeeQuery(ISqlExpression query, bool includeSoftDelete, bool includeFingerPrint)
        {
            Query = query;
            IncludeSoftDelete = includeSoftDelete;
            IncludeFingerPrint = includeFingerPrint;
        }
    }

    public class SyncEmployeeListQuery : QueryBase<PagingDataSource<SyncEmployeeDto>>,IInternalRequest
    {
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public DateTime? LastModifiedFrom { get; set; }
        public int RetailerId { get; set; }
    }
}
