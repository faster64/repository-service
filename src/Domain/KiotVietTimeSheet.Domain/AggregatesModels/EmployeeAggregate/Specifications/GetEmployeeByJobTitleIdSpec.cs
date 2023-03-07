using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetEmployeeByJobTitleIdSpec : ExpressionSpecification<Employee>
    {
        public GetEmployeeByJobTitleIdSpec(long jobTitleId)
            : base(e => e.JobTitleId == jobTitleId)
        { }
    }
}
