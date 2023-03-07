using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos
{
    public class GetBranchRevenue
    {
        public int RetailerId { get; set; }
        public IList<long> BranchIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
