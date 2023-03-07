using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetById
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetByIdQuery : QueryBase<PaysheetDto>
    {
        public long Id { get; set; }

        public bool IsReference { get; set;  }

        public GetPaysheetByIdQuery(long id, bool isReference = true)
        {
            Id = id;
            IsReference = isReference;
        }
    }
}
