using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByIdSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByIdSpec(long id)
        : base(c => c.Id == id)
        {
        }
    }
}
