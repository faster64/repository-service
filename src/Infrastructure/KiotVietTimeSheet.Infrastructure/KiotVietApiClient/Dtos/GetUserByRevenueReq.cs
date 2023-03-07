using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos
{
    public class GetUserByRevenueReq
    {
        public int RetailerId { get; set; }
        public List<int> BranchIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CommissionSetting { get; set; }
    }
}
