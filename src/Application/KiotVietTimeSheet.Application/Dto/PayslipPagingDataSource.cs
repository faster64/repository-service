using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PayslipPagingDataSource : PagingDataSource<PayslipDto>
    {
        public decimal TotalMainSalary { get; set; }
        public decimal TotalCommissionSalary { get; set; }
        public decimal TotalOvertimeSalary { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalRefund { get; set; }
        private decimal? _totalNeedPay;
        /// <summary>
        /// Còn cần trả
        /// </summary>
        public decimal TotalNeedPay
        {
            get
            {
                if (_totalNeedPay.HasValue) return _totalNeedPay.Value;
                _totalNeedPay = TotalNetSalary - TotalPayment - TotalRefund;
                _totalNeedPay = _totalNeedPay > 0 ? _totalNeedPay : 0;
                return _totalNeedPay.Value;
            }
            set => _totalNeedPay = value;

        }
    }
}
