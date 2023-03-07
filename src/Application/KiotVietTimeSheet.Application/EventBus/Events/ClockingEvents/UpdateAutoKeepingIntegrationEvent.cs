using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class UpdateAutoKeepingIntegrationEvent : IntegrationEvent
    {
        
        public List<Clocking> ListClockings { get; set; }

        public UpdateAutoKeepingIntegrationEvent(List<Clocking> listClockings)
        {
            ListClockings = listClockings;            
        }

        [JsonConstructor]
        public UpdateAutoKeepingIntegrationEvent()
        {

        }
    }
}
