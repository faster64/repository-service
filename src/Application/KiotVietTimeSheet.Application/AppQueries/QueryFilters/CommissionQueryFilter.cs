using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Application.AppQueries.QueryFilters
{
    public class CommissionQueryFilter : QueryFilterBase<Commission>
    {
        public List<long> IncludeCommissionIds { get; set; }
        public List<int> BranchIds { get; set; }

        public bool IncludeIsDeleted { get; set; }
        public bool IncludeInActive { get; set; }
        public string Keyword { get; set; }


    }
}
