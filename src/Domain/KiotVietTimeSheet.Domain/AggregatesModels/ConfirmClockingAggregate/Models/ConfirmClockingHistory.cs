using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models
{
    public class ConfirmClockingHistory : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
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

        public int TenantId { get; set; }

        public long ConfirmClockingId { get; set; }
        public ConfirmClocking ConfirmClocking { get; set; }

        public long ConfirmBy { get; set; }
        public DateTime ConfirmDate { get; set; }

        public byte StatusOld { get; set; }

        public byte StatusNew { get; set; }

        public string Note { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        #endregion

        #region Constructors
        public ConfirmClockingHistory()
        {
        }

        #endregion

        #region Methods

        public ConfirmClockingHistory(long confirmClockingId, long confirmBy, DateTime confirmDate, byte statusOld, byte statusNew, string note)
        {
            ConfirmClockingId = confirmClockingId;
            ConfirmBy = confirmBy;
            ConfirmDate = confirmDate;
            StatusOld = statusOld;
            StatusNew = statusNew;
            Note = note;

            AddDomainEvent(new CreatedConfirmClockingHistoryEvent(this));
        }

        public void Update(DateTime confirmDate, byte statusOld, byte statusNew, string note)
        {
            ConfirmDate = confirmDate;
            StatusOld = statusOld;
            StatusNew = statusNew;
            Note = note;

            AddDomainEvent(new UpdatedConfirmClockingHistoryEvent(this));
        }

        public void Delete()
        {
            var beforeChange = MemberwiseClone() as ConfirmClockingHistory;

            IsDeleted = true;
            AddDomainEvent(new DeletedConfirmClockingHistoryEvent(beforeChange));
        }

        #endregion
    }
}
