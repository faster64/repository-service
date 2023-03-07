using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetEmployeeByMobilePhoneSpec : ExpressionSpecification<Employee>
    {
        public GetEmployeeByMobilePhoneSpec(string mobilePhone)
            : base(e => e.MobilePhone == mobilePhone)
        { }
    }
}
