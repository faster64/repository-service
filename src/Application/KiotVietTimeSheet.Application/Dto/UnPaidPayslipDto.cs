using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class UnPaidPayslipDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TotalPayment { get; set; }
        public DateTime CreatedDate { get; set; }
        public long PaysheetId { get; set; }
    }
}
