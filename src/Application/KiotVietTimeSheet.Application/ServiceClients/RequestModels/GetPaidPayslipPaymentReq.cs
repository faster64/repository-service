
namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetPaidPayslipPaymentReq
    {
        public long PayslipId { get; set; }
        public long EmployeeId { get; set; }
        public string[] Includes { get; set; }
        public bool? IncludeUnAllocate { get; set; }
    }
}
