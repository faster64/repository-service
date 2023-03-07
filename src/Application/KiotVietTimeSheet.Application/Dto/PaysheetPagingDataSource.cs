using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class PaysheetPagingDataSource : PagingDataSource<PaysheetDto>
    {
        public decimal TotalNetSalary { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalNeedPay { get; set; }

        public decimal TotalRefund { get; set; }
    }
}
