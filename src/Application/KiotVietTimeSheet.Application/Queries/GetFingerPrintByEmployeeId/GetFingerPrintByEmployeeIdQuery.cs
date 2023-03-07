using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrintByEmployeeId
{
    public class GetFingerPrintByEmployeeIdQuery : QueryBase<FingerPrintDto>
    {
        public long EmployeeId { get; set; }
        public long? BranchId { get; set; }

        public GetFingerPrintByEmployeeIdQuery(long employeeId, long? branchId)
        {
            EmployeeId = employeeId;
            BranchId = branchId;
        }
    }
}
