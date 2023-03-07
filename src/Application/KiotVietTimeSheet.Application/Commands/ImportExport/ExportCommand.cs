using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.ImportExport
{
    public class ExportCommand : BaseCommand<ImportExportFileDto>
    {
        public string KvSessionId { get; set; }
        public object KVSession { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Columns { get; set; }
        public string Filters { get; set; }
        public string Revision { get; set; }
    }
}
