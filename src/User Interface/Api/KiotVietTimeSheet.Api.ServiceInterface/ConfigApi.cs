using System.Net;
using KiotVietTimeSheet.Api.ServiceModel;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using Microsoft.Extensions.Configuration;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceInterface
{
    public class ConfigApi : Service
    {
        public object Get(GetConfigTimeSheetJsReq req)
        {
            var configuration = HostContext.Resolve<IConfiguration>();
            var config = new ConfigTimeSheetJsDto
            {
                TimeSheetJs = configuration.GetSection("TimeSheetJs:Primary").Get<string>(),
                TimeSheetJsBackup = configuration.GetSection("TimeSheetJs:Backup").Get<string>(),
                CacheInSeconds = configuration.GetSection("TimeSheetJs:CacheInSeconds").Get<int>()
            };

            if (string.IsNullOrWhiteSpace(config.TimeSheetJs)) config.TimeSheetJs = "https://cdn-app.kiotviet.vn/timesheet/widget/timesheet-widget.bundle-1.0.42.js";
            if (string.IsNullOrWhiteSpace(config.TimeSheetJsBackup)) config.TimeSheetJsBackup = "https://d2q42mb8krc1g8.cloudfront.net/timesheet/widget/timesheet-widget.bundle-1.0.42.js";

            Response.StatusCode = (int)HttpStatusCode.OK;
            return new Response<ConfigTimeSheetJsDto>
            {
                Message = string.Empty,
                Result = config
            };
        }
    }
}
