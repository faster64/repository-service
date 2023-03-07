using System.Collections.Generic;

namespace KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos
{
    public class GetTotalPaymentByPayslipIdsReq
    {
        public int RetailerId { get; set; }
        public List<long> PayslipIds { get; set; }
    }
}
