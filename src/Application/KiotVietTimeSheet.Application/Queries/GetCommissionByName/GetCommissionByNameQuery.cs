using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionByName
{
    [RequiredPermission(TimeSheetPermission.Commission_Read)]
    public class GetCommissionByNameQuery : QueryBase<List<CommissionDto>>
    {
        public List<string> Names { get; set; }
        public GetCommissionByNameQuery(List<string> names)
        {
            Names = names;
        }
    }
}
