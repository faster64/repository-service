using System;

namespace KiotVietTimeSheet.Application.Logging
{
    public interface ILogger
    {
        void Info(string message);

        void Warn(string message);

        void Error(string message);

        void Debug(string message);

        void Error(string message, Exception ex);
    }
}
