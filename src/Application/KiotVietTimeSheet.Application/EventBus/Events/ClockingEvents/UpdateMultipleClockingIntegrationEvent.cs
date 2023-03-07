using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class UpdateMultipleClockingIntegrationEvent : IntegrationEvent
    {
        public List<Tuple<Clocking, Clocking>> ListClockings { get; set; }

        public UpdateMultipleClockingIntegrationEvent(List<Tuple<Clocking, Clocking>> listClockings)
        {
            ListClockings = listClockings;
        }

        [JsonConstructor]
        public UpdateMultipleClockingIntegrationEvent()
        {

        }
    }
}
