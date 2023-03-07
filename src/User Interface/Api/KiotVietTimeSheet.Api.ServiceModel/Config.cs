using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/config/timesheetjs", "GET")]
    public class GetConfigTimeSheetJsReq : IReturn<object>
    {
    }

    public class ConfigTimeSheetJsDto
    {
        public string TimeSheetJs { get; set; }
        public string TimeSheetJsBackup { get; set; }
        public int CacheInSeconds { get; set; }
    }
}