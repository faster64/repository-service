using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Application.AppQueries.QueryFilters
{
    public class CommissionDetailQueryFilter : QueryFilterBase<CommissionDetail>
    {
        public List<long> CommissionIds { get; set; }
    }
}
