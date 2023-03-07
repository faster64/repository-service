using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Specifications;
using KiotVietTimeSheet.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef.Repositories
{
    public class GpsInfoWriteOnlyRepository : EfBaseWriteOnlyRepository<GpsInfo>, IGpsInfoWriteOnlyRepository
    {
        public GpsInfoWriteOnlyRepository(EfDbContext db, 
            IAuthService authService, 
            ILogger<GpsInfoWriteOnlyRepository> logger)
           : base(db, authService, logger)
        {
            
        }

        public async Task<string> GetNewQrKey()
        {
            var qrKey = string.Empty;
            var loopCount = 0;
            var isExist = true;

            while (isExist && loopCount <= 100)
            {
                qrKey = Globals.RandomString(16);
                isExist = await AnyBySpecificationAsync(new FindGpsInfoByQrKeySpec(qrKey));
                loopCount++;
            }

            if (isExist) throw new KvTimeSheetException("Can not get new qr key.");

            return qrKey;
        }

        public async Task<string> ChangeQrKey(long id, Func<GpsInfo, Task> actionAudit = null)
        {
            var newQrCode = await GetNewQrKey();
            var gpsInforCur = await FindByIdAsync(id);
            gpsInforCur.QrKey = newQrCode;
            Update(gpsInforCur);
            await actionAudit(gpsInforCur);
            await UnitOfWork.CommitAsync();
            return newQrCode;
        }
    }
}