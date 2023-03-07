namespace KiotVietTimeSheet.Application.Dto
{
    public class FingerMachineDto
    {
        public long Id { get; set; }
        public int? BranchId { get; set; }
        public string MachineName { get; set; }
        public string MachineId { get; set; }
        public int Status { get; set; }
        public string Vendor { get; set; }
        public string Note { get; set; }
        public string IpAddress { get; set; }
        public int? Commpass { get; set; }
        public int? Port { get; set; }
        public int? ConnectionType { get; set; }

    }
}
