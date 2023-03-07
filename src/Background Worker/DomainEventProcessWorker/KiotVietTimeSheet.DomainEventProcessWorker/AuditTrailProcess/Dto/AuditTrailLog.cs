using System;

namespace KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Dto
{
    public class AuditTrailLog
    {
        public int FunctionId { get; set; }
        public int Action { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long UserId { get; set; }
    }
}
