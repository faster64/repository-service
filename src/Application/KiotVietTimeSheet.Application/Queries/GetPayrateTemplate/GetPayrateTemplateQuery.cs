using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPayrateTemplate
{
    [RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Read)]
    public class GetPayrateTemplateQuery : QueryBase<PagingDataSource<PayRateFormDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetPayrateTemplateQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
