using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipPenalizeDto
    {
        public long Id { get; set; }
        public long PayslipId { get; set; }
        public long PenalizeId { get; set; }
        public string PenalizeName { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public long EmployeeId { get; set; }
        public int MoneyType { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; }
        public long DeductionId { get; set; }
    }
}
