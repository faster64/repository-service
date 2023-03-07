using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class EmployeePaymentDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// Số tiền của phiếu
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public string Method { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public long? PayslipId { get; set; }
        public string Description { get; set; }
        public DateTime? TransDate { get; set; }
        public long EmployeeId { get; set; }
        public byte EmployeePaymentStatus { get; set; }
        public int AccountId { get; set; }
    }
}
