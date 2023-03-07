using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipWithTotalPagingDataSource : PagingDataSource<PayslipDto>
    {
        public decimal TotalMainSalary { get; set; }
        public decimal TotalCommissionSalary { get; set; }
        public decimal TotalOvertimeSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalImprest { get; set; }
        public decimal TotalDebt { get; set; }
    }
}
