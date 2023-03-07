using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetByIdForBgTask
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetByIdForBgTaskQuery : QueryBase<PaysheetDto>
    {
        public long Id { get; set; }

        public bool IsReference { get; set;  }

        public GetPaysheetByIdForBgTaskQuery(long id, bool isReference = true)
        {
            Id = id;
            IsReference = isReference;
        }
    }
}
