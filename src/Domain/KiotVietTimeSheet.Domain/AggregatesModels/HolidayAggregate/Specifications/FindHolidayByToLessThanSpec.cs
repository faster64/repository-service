using System;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByToLessThanSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByToLessThanSpec(DateTime dateTime)
            : base(c => c.To < dateTime)
        {
        }
    }
}
