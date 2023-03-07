using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetProfilePicturesByEmployeeIdSpec : ExpressionSpecification<EmployeeProfilePicture>
    {
        public GetProfilePicturesByEmployeeIdSpec(long employeeId)
           : base(entity => entity.EmployeeId == employeeId)
        {

        }
    }
}
