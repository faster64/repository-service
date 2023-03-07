using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByIds
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeByIdsQuery : QueryBase<List<EmployeeDto>>
    {
        public List<long> Ids { get; set; }

        public GetEmployeeByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
