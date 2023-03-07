using System;
using System.Collections.Generic;
using System.Text;

namespace KiotVietTimeSheet.Infrastructure.Configuration.ConfigurationModels
{
    public class MessageTemplateWriteLogger
    {
        public string KvSessionId { get; set; }
        public string RetailerCode { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long GroupId { get; set; }
        public string Description { get; set; }
        public string CurrentLang { get; set; }
        public int IndustryId { get; set; }
        public bool UserIsAdmin { get; set; }
    }
}
