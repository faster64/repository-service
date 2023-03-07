using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.Common
{
    public class ProductRevenue
    {
        public string UniqueId { get; set; } = Guid.NewGuid().ToString("N");
        public long? EmployeeId { get; set; }
        public int BranchId { get; set; }
        public long ProductId { get; set; }
        public string CategoryIds { get; set; }
        public decimal TotalCommission { get; set; }
        public double Quantity { get; set; }
        public string BillCode { get; set; }
        public DateTime? DateOfPayment { get; set; }
        public int TotalEmployee { get; set; }
    }
}