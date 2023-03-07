using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models
{
    public class PayslipPenalize : BaseEntity,
        IEntityIdlong,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate
    {
        public long Id { get; set; }
        public long PayslipId { get; set; }
        public long PenalizeId { get; set; }
        public string PenalizeName { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public int MoneyType { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DeletedBy { get; set; }
        public bool IsActive { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }

        public PayslipPenalize(
            long payslipId,
            long penalizeId,
            decimal value,
            int timesCount,
            int moneyType,
            bool isActive
            )
        {
            PayslipId = payslipId;
            PenalizeId = penalizeId;
            Value = value;
            TimesCount = timesCount;
            MoneyType = moneyType;
            IsActive = isActive;
        }
    }
}
