using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipPaymentAllocationDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public string Method { get; set; }
        public int? AccountId { get; set; }
        public byte Status { get; set; }
        public long? UserId { get; set; }
        public DateTime TransDate { get; set; }
        public long EmployeeId { get; set; }
        public string Description { get; set; }
        public long PayslipId { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
    }
}
