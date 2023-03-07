using ServiceStack;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/payRate",
        "GET",
        Summary = "Lấy thông tin mức lương của nhân viên",
        Notes = "")
    ]
    public class GetPayRateByEmployeeIdReq : QueryDb<PayRate>, IReturn<object>
    {
        public long EmployeeId { get; set; }
    }

    [Route("/payRate/{Id}",
        "GET",
        Summary = "Lấy thông tin mức lương theo id truyền vào",
        Notes = "")
    ]
    public class GetPayRateByIdReq : IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/payRate/get-by-template",
        "GET",
        Summary = "Lấy danh sách ",
        Notes = "")
    ]
    public class GetPayRateByTemplateIdReq : QueryDb<PayRate>, IReturn<object>
    {
        public long PayRateTemplateId { get; set; }
    }
}
