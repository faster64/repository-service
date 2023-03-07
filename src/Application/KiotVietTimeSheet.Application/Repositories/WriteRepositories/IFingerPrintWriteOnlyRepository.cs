using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IFingerPrintWriteOnlyRepository : IBaseWriteOnlyRepository<FingerPrint>
    {
        Task<List<FingerPrint>> FindByIdWithoutPermission(long employeeId);
    }
}
