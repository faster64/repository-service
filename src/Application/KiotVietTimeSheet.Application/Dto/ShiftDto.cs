namespace KiotVietTimeSheet.Application.Dto
{
    public class ShiftDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long From { get; set; }
        public long OldFrom { get; set; }
        public long To { get; set; }
        public long OldTo { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public long CheckInBefore { get; set; }
        public long CheckOutAfter { get; set; }
    }
}
