using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetAutoGenerateClockingSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetAutoGenerateClockingSpec(int quantityDay)
            : base(x =>
                !x.IsDeleted
                && x.IsRepeat == true
                && x.TimeSheetStatus == (byte) TimeSheetStatuses.Created
                && x.AutoGenerateClockingStatus == (byte) AutoGenerateClockingStatus.Auto
                && x.EndDate >= DateTime.Now.Date
                && x.EndDate <= DateTime.Now.Date.AddDays(quantityDay))
        {
        }
    }
}
