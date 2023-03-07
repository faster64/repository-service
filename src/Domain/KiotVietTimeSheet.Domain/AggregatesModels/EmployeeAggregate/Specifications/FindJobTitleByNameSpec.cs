using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindJobTitleByNameSpec : ExpressionSpecification<JobTitle>
    {
        public FindJobTitleByNameSpec(string name)
            : base(e => e.Name.Equals(name.ToPerfectString()))
        { }
    }
}
