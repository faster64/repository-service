using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByFromLessThanOrEqualSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByFromLessThanOrEqualSpec(DateTime dateTime)
            : base(c => c.From <= dateTime.Date)
        {
        }
    }
}
