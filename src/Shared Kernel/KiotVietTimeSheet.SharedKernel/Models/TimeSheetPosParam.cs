using System;

namespace KiotVietTimeSheet.SharedKernel.Models
{
    public class TimeSheetPosParam
    {
        public DateTime? TimesheetStartTrialDate { get; set; }
        public DateTime? TimesheetExpiredDate { get; set; }
        public int? ParameterType { get; set; }
        public int? BlockUnit { get; set; }
        public bool IsActive { get; set; }
    }
}
