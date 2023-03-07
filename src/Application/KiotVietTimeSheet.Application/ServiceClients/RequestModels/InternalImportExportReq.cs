using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class InternalImportExportReq
    {
        public int RetailerId { get; set; }
        public long UserId { get; set; }
        public ImportExportFile ImportExportFile { get; set; }
        public int BranchId { get; set; }
    }
}