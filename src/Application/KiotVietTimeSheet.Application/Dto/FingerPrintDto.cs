namespace KiotVietTimeSheet.Application.Dto
{
    public class FingerPrintDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public string FingerCode { get; set; }
        public long? EmployeeId { get; set; }
        public string FingerName { get; set; }
        public int FingerNo { get; set; }
        public string MachineId { get; set; }
    }
}
