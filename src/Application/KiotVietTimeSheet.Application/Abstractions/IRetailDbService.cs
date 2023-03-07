using System.Threading.Tasks;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public interface IRetailDbService
    {
        Task<bool> HasDuplicateTaskExportBySessionId(string sessionId, string revision, string type);
        Task<bool> HasDuplicateTaskImportBySessionId(string sessionId, string type);
        Task<ImportExportFile> AddImportExportFile(ImportExportFile entity);
    }
}
