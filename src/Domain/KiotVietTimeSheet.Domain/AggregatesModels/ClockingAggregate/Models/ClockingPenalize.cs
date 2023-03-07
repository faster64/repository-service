using System;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models
{
    public class ClockingPenalize : BaseEntity,
        IEntityIdlong,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ITenantId,
        ISoftDelete
    {
        public static ClockingPenalize CreateInstance()
        {
            return new ClockingPenalize();
        }
        private ClockingPenalize() { }

        public long Id { get; set; }
        public long ClockingId { get; set; }
        public long EmployeeId { get; set; }
        public long PenalizeId { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }

        public Clocking Clocking { get; set; }
        public Penalize Penalize { get; set; }
        public int MoneyType { get; set; }
        /// <summary>
        /// Trạng thái trả lương <see cref="ClockingPaymentStatuses"/>
        /// </summary>
        public byte ClockingPaymentStatus { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }

        public void UpdateClockingPaymentStatus(byte clockingPaymentStatus)
        {
            ClockingPaymentStatus = clockingPaymentStatus;
        }

        public void Update(int timesCount, decimal value, long penalizeId)
        {
            TimesCount = timesCount;
            Value = value;
            PenalizeId = penalizeId;
        }
    }
}
