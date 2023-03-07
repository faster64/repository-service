using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public abstract class BaseObjectEntity : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,//NOSONAR
        ITenantId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete,
        ICacheable
    {
        #region Properties
        public long Id { get; set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsActive { get; protected set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        #endregion

    }
}
