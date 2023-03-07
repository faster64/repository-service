using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/mobile/config", "GET", Summary = "Lấy timesheet config", Notes = "")]
    public class GetTimesheetConfigForMobileReq : IReturn<object>
    {
    }

    [Route("/mobile/branchs", "GET", Summary = "Lấy danh sách chi nhánh", Notes = "")]
    public class GetBranchsForMobileReq : IReturn<object>
    {
    }

    [Route("/mobile/user", "GET", Summary = "Lấy lang", Notes = "")]
    public class GetUserMobileReq : IReturn<object>
    {
    }
}