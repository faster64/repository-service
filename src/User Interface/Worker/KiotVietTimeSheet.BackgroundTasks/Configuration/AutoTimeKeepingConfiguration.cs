using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.BackgroundTasks.Configuration
{
    public class AutoTimeKeepingConfiguration
    {
        public bool IsEnable { get; set; }
        public string TimeAmplitude { get; set; }
        public List<int> GroupIds { get; set; }
        public List<int> IncludeTenantIds { get; set; }
        public List<int> ExcludeTenantIds { get; set; }        
        public string Schedule { get; set; }
        public string ReTrySchedule { get; set; }
        public int RetryDayAgo { get; set; }
        private int? _beforeTimeAmplitude;
        public int GetBeforeTimeAmplitude()
        {
            try
            {
                if (!_beforeTimeAmplitude.HasValue)
                    _beforeTimeAmplitude = Convert.ToInt32(TimeAmplitude.Split(',')[0]);

                return _beforeTimeAmplitude.Value;
            }
            catch(Exception epx)
            {
                Console.WriteLine(epx.Message);
                return 1440;
            }
        }
        private int? _afterTimeAmplitude;
        public int GetAfterTimeAmplitude()
        {
            try
            {
                if(!_afterTimeAmplitude.HasValue)
                    _afterTimeAmplitude = Convert.ToInt32(TimeAmplitude.Split(',')[1]);

                return _afterTimeAmplitude.Value;
            }
            catch (Exception epx)
            {
                Console.WriteLine(epx.Message);
                return 30;
            }
        }
    }
}
