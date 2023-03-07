using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models
{
    public class PayslipClockingPenalize : BaseEntity,
        IEntityIdlong,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate
    {
        public PayslipClockingPenalize() { } //NOSONAR

        public long Id { get; set; }
        public long PayslipId { get; set; }
        public long ClockingId { get; set; }
        public long PenalizeId { get; set; }
        public string PenalizeName { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public int MoneyType { get; set; }
        public DateTime ClockingPenalizeCreated { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }
        public long? ShiftId { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }

        public PayslipClockingPenalize(
            long payslipId,
            long clockingId,
            long penalizeId,
            decimal value,
            int timesCount,
            int moneyType,
            DateTime clockingPenalizeCreated,
            long? shiftId
            )
        {
            PayslipId = payslipId;
            ClockingId = clockingId;
            PenalizeId = penalizeId;
            Value = value;
            TimesCount = timesCount;
            MoneyType = moneyType;
            ClockingPenalizeCreated = clockingPenalizeCreated;
            ShiftId = shiftId;
        }
    }
}
