namespace KiotVietTimeSheet.SharedKernel.ConfigModels
{
    public class FtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Directory { get; set; }
    }
}
