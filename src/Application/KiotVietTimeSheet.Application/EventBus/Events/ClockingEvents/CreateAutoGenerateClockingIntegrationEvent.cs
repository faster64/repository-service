using System;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class CreateAutoGenerateClockingIntegrationEvent : IntegrationEvent
    {
        public int QuantityDayCondition { get; set; }
        public int QuantityDayAdd { get; set; }
        public int EmployeeWorkingInDay { get; set; }
        public DateTime HandleExpiredAt { get; set; }

        [JsonConstructor]
        public CreateAutoGenerateClockingIntegrationEvent()
        {
        }
    }
}
