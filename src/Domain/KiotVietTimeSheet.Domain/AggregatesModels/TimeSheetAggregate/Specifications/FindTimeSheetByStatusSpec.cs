using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByStatusSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByStatusSpec(byte status)
            : base(ts => ts.TimeSheetStatus == status)
        {
        }
    }
}
