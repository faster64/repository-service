using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByAccountSecretKeySpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByAccountSecretKeySpec(string accountSecretKey)
            : base(e => e.AccountSecretKey == accountSecretKey)
        { }
    }
}
