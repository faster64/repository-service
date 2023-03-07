using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByIdSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByIdSpec(long id)
            : base(h => h.Id == id) { }
    }
}
