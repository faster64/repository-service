using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByFromLessThanSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByFromLessThanSpec(DateTime dateTime)
            : base(c => c.From < dateTime)
        {
        }
    }
}
