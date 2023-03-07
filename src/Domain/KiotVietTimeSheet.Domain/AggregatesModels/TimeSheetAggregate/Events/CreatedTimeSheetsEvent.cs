using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events
{
    public class CreatedTimeSheetsEvent : DomainEvent
    {
        public List<TimeSheet> TimeSheets { get; set; }
    }
}
