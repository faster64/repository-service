using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    public class TimeSheetSendInformation
    {
        [Route("/retailer-information", "POST", Summary = "",
            Notes = "Gửi thông tin người muốn sử dụng Timesheet")]
        public class CreateCustomerInformation : IReturn<string>
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public int? EmployeeNumber { get; set; }

        }
    }
}
