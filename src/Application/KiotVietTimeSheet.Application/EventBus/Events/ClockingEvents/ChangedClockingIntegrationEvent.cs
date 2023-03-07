using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class ChangedClockingIntegrationEvent : IntegrationEvent
    {
        public TimeSheetDto TimeSheetDto { get; set; }
        public TimeSheet TimeSheet { get; set; }
        public bool ForAllClockings { get; set; }
        public bool HasChange { get; set; }

        [JsonConstructor]
        public ChangedClockingIntegrationEvent()
        {
        }

        public ChangedClockingIntegrationEvent(TimeSheetDto timeSheetDto, TimeSheet timeSheet, bool forAllClockings, bool hasChange)
        {
            TimeSheetDto = timeSheetDto;
            TimeSheet = timeSheet;
            ForAllClockings = forAllClockings;
            HasChange = hasChange;
        }
    }
}