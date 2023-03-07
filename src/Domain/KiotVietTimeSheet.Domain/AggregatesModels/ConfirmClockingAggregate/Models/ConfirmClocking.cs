using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models
{
    public class ConfirmClocking : BaseEntity,
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

        public long GpsInfoId { get; set; }
        public GpsInfo GpsInfo { get; set; }

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime CheckTime { get; set; }

        public byte Type { get; set; }

        public byte Status { get; set; }

        public string Note { get; set; }

        public string IdentityKeyClocking { get; set; }

        public string Extra { get; set; }   // store json string

        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public List<ConfirmClockingHistory> ConfirmClockingHistories { get; set; }
        #endregion

        #region Constructors
        public ConfirmClocking()
        {
        }

        #endregion

        #region Methods

        public ConfirmClocking(long gpsInfoId, long employeeId, DateTime checkTime, byte type, byte status, string note)
        {
            GpsInfoId = gpsInfoId;
            EmployeeId = employeeId;
            CheckTime = checkTime;
            Type = type;
            Status = status;
            Note = note;

            AddDomainEvent(new CreatedConfirmClockingEvent(this));
        }

        public void Update(DateTime checkTime, byte type, byte status, string note)
        {
            CheckTime = checkTime;
            Type = type;
            Status = status;
            Note = note;

            AddDomainEvent(new UpdatedConfirmClockingEvent(this));
        }

        public void Delete()
        {
            var beforeChange = MemberwiseClone() as ConfirmClocking;

            IsDeleted = true;
            AddDomainEvent(new DeletedConfirmClockingEvent(beforeChange));
        }

        #endregion
    }
}
