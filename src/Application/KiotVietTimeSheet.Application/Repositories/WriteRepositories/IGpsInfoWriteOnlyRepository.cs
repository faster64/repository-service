using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IGpsInfoWriteOnlyRepository : IBaseWriteOnlyRepository<GpsInfo>
    {
        Task<string> GetNewQrKey();

        Task<string> ChangeQrKey(long id, Func<GpsInfo, Task> actionAudit = null);
    }
}