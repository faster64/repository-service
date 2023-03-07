using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionById
{
    [RequiredPermission(TimeSheetPermission.Deduction_Read)]
    public class GetDeductionByIdQuery : QueryBase<DeductionDto>
    {
        public long Id { get; set; }

        public GetDeductionByIdQuery(long id)
        {
            Id = id;
        }
    }
}
