using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByToLessThanOrEqualSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByToLessThanOrEqualSpec(DateTime dateTime)
            : base(h => h.To < dateTime.Date.AddDays(1))
        {
        }
    }
}
