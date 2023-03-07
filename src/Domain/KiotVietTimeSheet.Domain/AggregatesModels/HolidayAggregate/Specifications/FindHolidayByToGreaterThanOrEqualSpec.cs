using System;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByToGreaterThanOrEqualSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByToGreaterThanOrEqualSpec(DateTime dateTime)
            : base(c => c.To >= dateTime.Date)
        {
        }
    }
}
