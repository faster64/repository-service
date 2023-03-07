using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Application.Queries.GetBranchsForMobile
{
    [RequiredPermission(TimeSheetPermission.Branch_Read)]
    public class GetBranchsForMobileQuery : QueryBase<List<BranchMobileDto>>
    {

        public GetBranchsForMobileQuery()
        {

        }
    }
}
