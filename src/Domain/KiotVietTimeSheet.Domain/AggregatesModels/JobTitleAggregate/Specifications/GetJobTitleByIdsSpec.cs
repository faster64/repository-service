using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Specifications
{
    public class GetJobTitleByIdsSpec : ExpressionSpecification<JobTitle>
    {
        public GetJobTitleByIdsSpec(ICollection<long?> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
