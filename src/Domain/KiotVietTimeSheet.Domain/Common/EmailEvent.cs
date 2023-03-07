namespace KiotVietTimeSheet.Domain.Common
{
    public class EmailEvent
    {
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string UserCertify { get; set; }
        public string PasswordCertify { get; set; }
        public string SenderEmail { get; set; }
        public string SendTo { get; set; }
        public string BccEmail { get; set; }
        public string ReplyTo { get; set; }
        public string SenderName { get; set; }
        public string ReplyToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
