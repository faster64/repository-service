using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipClockingPenalizeDto
    {
        public long Id { get; set; }
        public long PayslipId { get; set; }
        public long ClockingId { get; set; }
        public long PenalizeId { get; set; }
        public string PenalizeName { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public long EmployeeId { get; set; }
        public int MoneyType { get; set; }
        public DateTime ClockingPenalizeCreated { get; protected set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? ShiftId { get; set; }
        public string ShiftName { get; set; }
    }
}
