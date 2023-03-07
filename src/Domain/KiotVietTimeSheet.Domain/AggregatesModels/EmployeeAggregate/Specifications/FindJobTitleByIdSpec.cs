using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindJobTitleByIdSpec : ExpressionSpecification<JobTitle>
    {
        public FindJobTitleByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
