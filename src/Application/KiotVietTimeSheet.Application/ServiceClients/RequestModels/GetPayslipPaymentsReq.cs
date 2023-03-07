using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetPayslipPaymentsReq
    {
        public List<long> PayslipIds { get; set; }
        public long? PaysheetId { get; set; }
        public byte? Status { get; set; }
        public string[] Includes { get; set; }
        public bool? WithAllocation { get; set; }
    }
}
