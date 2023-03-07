using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetTimeSheetProductCommissionReq
    {
        public int? CategoryId { get; set; }
        public bool IncludeInActive { get; set; }
        public List<long> CommissionIds { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public string ProductCodeKeyword { get; set; }
        public string ProductNameKeyword { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
