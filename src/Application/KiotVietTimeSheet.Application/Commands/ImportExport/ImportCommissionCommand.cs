using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.Web;

namespace KiotVietTimeSheet.Application.Commands.ImportExport
{
    [RequiredPermission(TimeSheetPermission.Commission_Import)]
    public class ImportCommissionCommand : BaseCommand<ImportExportFileDto>
    {
        public string KvSessionId { get; set; }
        public object KVSession { get; set; }
        public IHttpFile[] Files { get; set; }
        public string BearerToken { get; set; }
    }
}
