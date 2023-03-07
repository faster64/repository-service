using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeAvailable
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeAvailableQuery : QueryBase<PagingDataSource<EmployeeDto>>
    {
        public int BranchId { get; set; }
        public long WithoutId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string Keyword { get; set; }
        public GetEmployeeAvailableQuery(int branchId, long withoutId, DateTime start, DateTime end, int? skip, int? take, string keyword)
        {
            BranchId = branchId;
            WithoutId = withoutId;
            Start = start;
            End = end;
            Skip = skip;
            Take = take;
            Keyword = keyword;
        }
    }
}
