using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByFromYearEqualSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByFromYearEqualSpec(DateTime startDayYear)
            : base(c => c.From >= startDayYear)
        {
        }
    }
}
