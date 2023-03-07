using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using KiotVietTimeSheet.SharedKernel.Auth;
using Serilog.Context;
using ServiceStack;
using ILogger = Serilog.ILogger;

namespace KiotVietTimeSheet.Infrastructure.Logging.Serilog
{
    public class SerilogFactory : Application.Logging.ILogger
    {
        private ILogger Logger { get; set; }
        private IConfiguration Configuration { get; set; }

        public SerilogFactory(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigLogger();
        }

        public void Debug(string message)
        {
            PushPropertiesToSeriLog();
            Logger.Debug(ProcessMessage(message));
        }

        public void Error(string message)
        {
            PushPropertiesToSeriLog();
            Logger.Error(ProcessMessage(message));
        }

        public void Error(string message, Exception ex)
        {
            PushPropertiesToSeriLog(ex);
            Logger.Error(ProcessMessage(message));
        }

        public void Info(string message)
        {
            PushPropertiesToSeriLog();
            Logger.Information(ProcessMessage(message));
        }

        public void Warn(string message)
        {
            PushPropertiesToSeriLog();
            Logger.Warning(ProcessMessage(message));
        }

        private void ConfigLogger()
        {
            var logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
            Logger = logger;
        }

        /// <summary>
        /// Thêm 1 số property để xử lý serilog trong appsettings
        /// </summary>
        private void PushPropertiesToSeriLog(Exception ex = null)
        {
            var context = HostContext.TryResolve<ExecutionContext>();
            if (context == null) return;
            LogContext.PushProperty("BranchId", context.BranchId);
            LogContext.PushProperty("RetailerId", context.TenantId);
            LogContext.PushProperty("UserId", context.User?.Id ?? 0);
            LogContext.PushProperty("UserName", context.User?.UserName ?? string.Empty);
            LogContext.PushProperty("RetailerCode", context.TenantCode);
            LogContext.PushProperty("GroupId", context.User?.GroupId ?? 0);
            LogContext.PushProperty("KvSessionId", context.User?.KvSessionId ?? string.Empty);
            LogContext.PushProperty("IndustryId", context.User?.IndustryId ?? 0);
            LogContext.PushProperty("HttpMethod", context.HttpMethod);
            if (ex != null)
            {
                var baseException = ex.GetBaseException();
                if (!string.IsNullOrEmpty(baseException.StackTrace))
                {
                    var stackTrace = baseException.StackTrace.Replace(Environment.NewLine, " ");
                    LogContext.PushProperty("StackTrace", stackTrace);
                }

            }
        }

        private string ProcessMessage(string message)
        {
            return message.Replace(Environment.NewLine, " ");
        }
    }
}
