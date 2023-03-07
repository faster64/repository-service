using System;
namespace KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess
{
    public class AuditProcessException : Exception
    {
        public AuditProcessException(string msg) : base(msg)
        {
        }
    }
}
