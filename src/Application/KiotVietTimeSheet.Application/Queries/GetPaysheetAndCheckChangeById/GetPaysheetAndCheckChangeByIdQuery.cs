using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeById
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetAndCheckChangeByIdQuery : QueryBase<PaysheetDto>
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public int KvSessionBranchId { get; set; }

        public GetPaysheetAndCheckChangeByIdQuery(long id, int branchId, int kvSessionBranchId)
        {
            Id = id;
            BranchId = branchId;
            KvSessionBranchId = kvSessionBranchId;
        }
    }
}
