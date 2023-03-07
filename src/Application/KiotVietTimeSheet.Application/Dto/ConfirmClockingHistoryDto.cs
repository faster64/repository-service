using System;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ConfirmClockingHistoryDto
    {
        public long Id { get; set; }

        public int TenantId { get; set; }

        public long ConfirmClockingId { get; set; }
        public ConfirmClockingDto ConfirmClocking { get; set; }

        public long ConfirmBy { get; set; }
        public DateTime ConfirmDate { get; set; }

        public byte StatusOld { get; set; }

        public byte StatusNew { get; set; }

        public string Note { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }

        public string Content { get; set; }
        public string Reason { get; set; }
        public string CheckTimeFormat { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
