using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.Dtos
{
    public class GetTotalPaymentByPayslipIdsReq
    {
        public int RetailerId { get; set; }
        public List<long> PayslipIds { get; set; }
    }
}
