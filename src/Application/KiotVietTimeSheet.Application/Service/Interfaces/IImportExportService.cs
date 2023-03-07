using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Service.Interfaces
{
    public interface IImportExportService
    {
        Task<ImportExportExecutionContext> GetExecutionContext();
        Task<ImportExportSession> GetSession(object kvSession);
    }
}
