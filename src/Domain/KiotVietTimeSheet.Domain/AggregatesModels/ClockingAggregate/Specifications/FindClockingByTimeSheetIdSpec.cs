using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByTimeSheetIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByTimeSheetIdSpec(long timeSheetId)
            : base(c => c.TimeSheetId == timeSheetId)
        { }
    }
}
