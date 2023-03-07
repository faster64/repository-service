using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByBranchId
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeByBranchIdQuery : QueryBase<List<EmployeeDto>>
    {
        public int BranchId { get; set; }

        public int? TypeSearch { get; set; }
        public GetEmployeeByBranchIdQuery(int branchId, int? typeSearch)
        {
            BranchId = branchId;
            TypeSearch = typeSearch;
        }
    }
}
