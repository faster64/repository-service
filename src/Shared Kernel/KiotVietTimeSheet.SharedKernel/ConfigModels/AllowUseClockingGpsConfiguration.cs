using System.Collections.Generic;

namespace KiotVietTimeSheet.SharedKernel.ConfigModels
{
    public class AllowUseClockingGpsConfiguration
    {
        public bool IsEnableFilter { get; set; }
        public List<int> IncludeTenantIds { get; set; }
        public List<int> ExcludeTenantIds { get; set; }
    }
}
