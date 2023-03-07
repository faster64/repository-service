using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ConfirmClockingDto
    {
        public long Id { get; set; }

        public int TenantId { get; set; }

        public long GpsInfoId { get; set; }
        public GpsInfoDto GpsInfo { get; set; }

        public long EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }

        public DateTime CheckTime { get; set; }

        public byte Type { get; set; }

        public byte Status { get; set; }

        public string Note { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public string Content { get; set; }
        public string Reason { get; set; }
        public string CheckTimeFormat { get; set; }

        public string Extra { get; set; }

        public List<ConfirmClockingHistoryDto> ConfirmClockingHistories { get; set; }
    }
}
