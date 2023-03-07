using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.Diagnostics;

namespace KiotVietTimeSheet.Infrastructure.AuditLog
{
    public class LogObject
    {
        public LogObject(ILogger logger, Guid id)
        {
            Id = id;
            Timer = Stopwatch.StartNew();
            StartTime = DateTime.Now.ToString("o");
            Logger = logger;
        }

        public Guid Id { get; set; }
        public string Action { get; set; }

        private Stopwatch Timer { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long Duration { get; set; }
        public int? RetailerId { get; set; }
        public int? BranchId { get; set; }
        public string RetailerCode { get; set; }
        public string Description { get; set; }
        public object RequestObject { get; set; }
        public object ResponseObject { get; set; }
        private ILogger Logger { get; set; }

        public void LogInfo(bool isLogTotalTime = false)
        {
            Timer.Stop();
            EndTime = DateTime.Now.ToString("o");

            Duration = Timer.ElapsedMilliseconds;

            RequestObject = ConvertObjectToJson(RequestObject);

            ResponseObject = ConvertObjectToJson(ResponseObject);

            Logger.LogInformation(this.ToSafeJson());
        }

        public void LogError(Exception ex = null)
        {
            Timer.Stop();

            EndTime = DateTime.Now.ToString("o");

            Duration = Timer.ElapsedMilliseconds;

            RequestObject = ConvertObjectToJson(RequestObject);

            ResponseObject = ConvertObjectToJson(ResponseObject);

            if (ex != null)
            {
                Logger.LogError(this.ToSafeJson(), ex);
            }
            else
            {
                Logger.LogError(this.ToSafeJson());
            }
        }

        private string ConvertObjectToJson(object obj)
        {
            if (obj != null && !(obj is string))
            {
                return obj.ToSafeJson();
            }

            return string.Empty;
        }
    }
}
