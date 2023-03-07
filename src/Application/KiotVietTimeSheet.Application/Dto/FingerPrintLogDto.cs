using System;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.Dto
{
    public class FingerPrintLogDto
    {
        public string FingerCode { get; set; }
        public DateTime CheckDateTime { get; set; }
        public string MachineId { get; set; }
        public long BranchId { get; set; }
        public string Uid { get; set; }
        public int Status { get; set; }
        public long? EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }
        [JsonIgnore]
        public bool IsSetDefaultCheckIn { get; set; } // Dùng làm param trong quá trình xử lý logic, không nhận giá trị từ client
    }
}
