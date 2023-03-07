using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;


namespace KiotVietTimeSheet.Application.Queries.GetPaysheetExistsPayslips
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public class GetPaysheetExistsPayslipsQuery : QueryBase<bool>
    {
        public long PaysheetId { get; set; }

        public GetPaysheetExistsPayslipsQuery(long paysheetId)
        {
            PaysheetId = paysheetId;
        }
    }
}
