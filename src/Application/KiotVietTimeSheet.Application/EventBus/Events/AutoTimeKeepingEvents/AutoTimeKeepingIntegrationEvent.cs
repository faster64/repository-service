using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.AutoTimeKeepingEvents
{
    public class AutoTimeKeepingIntegrationEvent : IntegrationEvent
    {
        public List<AutoTimeKeepingResult> FingerPrintLogs { get; set; }

        public AutoTimeKeepingIntegrationEvent(List<AutoTimeKeepingResult> autoTimeKeepingResults)
        {
            FingerPrintLogs = autoTimeKeepingResults;
        }

        [JsonConstructor]
        public AutoTimeKeepingIntegrationEvent()
        {

        }
    }
}
