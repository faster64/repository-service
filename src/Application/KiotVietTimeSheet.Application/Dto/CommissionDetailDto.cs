using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class CommissionDetailDto
    {
        public long Id { get; set; }
        public long CommissionId { get; set; }
        public string CommissionName { get; set; }
        public long ObjectId { get; set; }
        public string ProductName { get; set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueRatio { get; set; }
        public byte Type { get; set; }
    }
}
