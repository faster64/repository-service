using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetPayslipPaymentsValueIncludeAllocationReq
    {
        public List<long> EmployeeIds { get; set; }
    }
}
