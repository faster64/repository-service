using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class CreateMultipleClockingIntegrationEvent : IntegrationEvent
    {
        public TimeSheet TimeSheet { get; set; }
        public TimeSheetDto TimeSheetDto { get; set; }
        public List<Clocking> ListClockings { get; set; }
        public bool IsManual { get; set; }
        public bool ForAllClockings { get; set; }
        public bool HasChange { get; set; }
        public bool FromAuto { get; set; }

        public CreateMultipleClockingIntegrationEvent(List<Clocking> listClockings)
        {
            ListClockings = listClockings;
        }

        public CreateMultipleClockingIntegrationEvent(TimeSheetDto timeSheetDto, List<Clocking> listClockings, bool isManual = false)
        {
            TimeSheetDto = timeSheetDto;
            ListClockings = listClockings;
            IsManual = isManual;
        }

        public CreateMultipleClockingIntegrationEvent(TimeSheetDto timeSheetDtot, TimeSheet timeSheetOld, List<Clocking> listClockings, bool forAllClockings = false, bool hasChange = false, bool fromAuto = false)
        {
            TimeSheetDto = timeSheetDtot;
            TimeSheet = timeSheetOld;
            ListClockings = listClockings;
            ForAllClockings = forAllClockings;
            HasChange = hasChange;
            FromAuto = fromAuto;
        }

        [JsonConstructor]
        public CreateMultipleClockingIntegrationEvent()
        {
        }
    }
}