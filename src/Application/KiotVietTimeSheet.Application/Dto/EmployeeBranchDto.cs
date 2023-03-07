
namespace KiotVietTimeSheet.Application.Dto
{
    public class EmployeeBranchDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; protected set; }
        public long EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }
    }
}
