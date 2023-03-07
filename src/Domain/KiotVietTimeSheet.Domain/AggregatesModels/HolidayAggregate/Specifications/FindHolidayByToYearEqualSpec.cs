using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByToYearEqualSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByToYearEqualSpec(DateTime endDayYear)
            : base(c => c.To < endDayYear)
        {
        }
    }
}
