using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeMultipleBranch
{
    public class GetEmployeeMultipleBranchQuery : QueryBase<PagingDataSource<EmployeeDto>>
    {
        public List<int> BranchIds { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long?> DepartmentIds { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsIncludeDelete { get; set; }
        public string Keyword { get; set; }
        public List<long> EmployeeIds { get; set; }
        public GetEmployeeMultipleBranchQuery(
            List<int> branchIds,
            List<long> shiftIds,
            List<long?> departmentIds,
            bool? isActive,
            bool? isIncludeDelete,
            string keyword,
            List<long> employeeIds)
        {
            BranchIds = branchIds;
            ShiftIds = shiftIds;
            DepartmentIds = departmentIds;
            IsActive = isActive;
            IsIncludeDelete = isIncludeDelete;
            Keyword = keyword;
            EmployeeIds = employeeIds;
        }
    }
}
