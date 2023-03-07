using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPayrateTemplateById
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Read)]
    public class GetPayrateTemplateByIdQuery : QueryBase<PayRateFormDto>
    {
        public long Id { get; set; }

        public GetPayrateTemplateByIdQuery(long id)
        {
            Id = id;
        }
    }
}
