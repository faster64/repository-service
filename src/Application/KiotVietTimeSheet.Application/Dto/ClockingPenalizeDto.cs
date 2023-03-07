using System;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingPenalizeDto : BaseEntityByBaseInfo
    {
        public long ClockingId { get; set; }
        public long PenalizeId { get; set; }
        public decimal Value { get; set; }
        public int TimesCount { get; set; }
        public long EmployeeId { get; set; }
        public int MoneyType { get; set; }
        public PenalizeDto PenalizeDto { get; set; }
        public ClockingDto ClockingDto { get; set; }
        public long ShiftId { get; set; }
        public DateTime ClockingPenalizeCreated { get; set; }
    }
}
