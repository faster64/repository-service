using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetListEmployeeDebtReq
    {
        public List<long> EmployeeIds { get; set; }
    }
}
