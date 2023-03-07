using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetAndCheckChangeVersion
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetAndCheckChangeVersionQuery : QueryBase<bool>
    {
        public long Id { get; set; }
        public long Version { get; set; }

        public GetPaysheetAndCheckChangeVersionQuery(long id, long version)
        {
            Id = id;
            Version = version;
        }
    }
}
