using System.Collections.Generic;

namespace KiotVietTimeSheet.BackgroundTasks.Configuration
{
    public class ConfigMail
    {
        public string KiotMailServerList { get; set; }
        public IList<string> SendToWhenActiveTimeSheetEmails { get; set; }
        public string SendFromWhenActiveTimeSheetEmail { get; set; }
        public int KiotMailPort { get; set; }
        public bool KiotMailUseSsl { get; set; }
        public string KiotMailPasswordCertify { get; set; }
        public string KiotMailUsernameCertify { get; set; }
    }
}
