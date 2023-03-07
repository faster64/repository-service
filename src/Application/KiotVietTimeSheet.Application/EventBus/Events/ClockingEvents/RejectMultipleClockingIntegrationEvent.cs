using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class RejectMultipleClockingIntegrationEvent : IntegrationEvent
    {
        public TimeSheetDto TimeSheetDto { get; set; }
        public TimeSheet TimeSheet { get; set; }
        public List<Clocking> ListClockings { get; set; }
        public bool HasChange { get; set; }

        public RejectMultipleClockingIntegrationEvent(List<Clocking> listClockings)
        {
            ListClockings = listClockings;
        }

        public RejectMultipleClockingIntegrationEvent(TimeSheet timeSheet, List<Clocking> listClockings)
        {
            TimeSheet = timeSheet;
            ListClockings = listClockings;
        }

        public RejectMultipleClockingIntegrationEvent(TimeSheetDto timeSheetDto, TimeSheet timeSheet, List<Clocking> listClockings, bool hasChange = false)
        {
            TimeSheetDto = timeSheetDto;
            TimeSheet = timeSheet;
            ListClockings = listClockings;
            HasChange = hasChange;
        }

        [JsonConstructor]
        public RejectMultipleClockingIntegrationEvent()
        {

        }
    }
}
