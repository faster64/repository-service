using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeBranchByEmployeeIds
{
    public class GetEmployeeBranchByEmployeeIdsQuery : QueryBase<List<EmployeeBranchDto>>
    {
        public List<long> ListEmployeeId { get; set; }

        public GetEmployeeBranchByEmployeeIdsQuery(List<long> listEmployeeId)
        {
            ListEmployeeId = listEmployeeId;
        }
    }
}
