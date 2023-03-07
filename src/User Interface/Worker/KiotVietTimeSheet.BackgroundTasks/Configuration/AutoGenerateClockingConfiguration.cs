using System.Collections.Generic;

namespace KiotVietTimeSheet.BackgroundTasks.Configuration
{
    public class AutoGenerateClockingConfiguration
    {
        public bool IsEnable { get; set; }
        public List<int> GroupIds { get; set; }
        public List<int> IncludeTenantIds { get; set; }
        public List<int> ExcludeTenantIds { get; set; }
        public int IntervalMinute { get; set; }
        public int QuantityDayCondition { get; set; }
        public int QuantityDayAdd { get; set; }
        public int EmployeeWorkingInDay { get; set; }
    }
}
