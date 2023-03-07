using System;

namespace KiotVietTimeSheet.Application.ServiceClients.Dtos
{
    public class WriteAuditLogRequest
    {
        public int FunctionId { get; set; }

        public int Action { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public int RetailerId { get; set; }

        public int BranchId { get; set; }

        public long UserId { get; set; }
    }
}
