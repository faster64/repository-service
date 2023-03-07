using System;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public abstract class BaseEntityByBaseInfo
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
