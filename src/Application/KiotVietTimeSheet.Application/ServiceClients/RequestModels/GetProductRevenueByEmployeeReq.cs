using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetProductRevenueByEmployeeReq
    {
        public int tenantId { get; set; }
        public List<int> branchIds { get; set; }
        public List<long> userIds { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}