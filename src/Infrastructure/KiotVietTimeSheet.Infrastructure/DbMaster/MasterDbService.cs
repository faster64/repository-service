using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.DbMaster
{
    public class MasterDbService : IMasterDbService
    {
        private readonly DbMasterContext _db;
        private readonly ICacheClient _cacheClient;
        private const string RetailerCacheKey = "TimeSheet-worker:Retailers:{0}";
        private const string RetailerGroupCacheKey = "TimeSheet-worker:KvGroup:{0}";

        public MasterDbService(DbMasterContext db, ICacheClient cacheClient)
        {
            _db = db;
            _cacheClient = cacheClient;
        }

        public Task<KvRetailer> GetRetailerAsync(int retailerId)
        {
            return _cacheClient.GetAndSetWithExpireAsync(string.Format(RetailerCacheKey, retailerId), (key) =>
            {
                return _db.KvRetailer.FirstOrDefaultAsync(r => r.Id == retailerId);
            },TimeSpan.FromMinutes(5));
        }

        public Task<KvGroup> GetGroupAsync(int groupId)
        {
            return _cacheClient.GetAsync(string.Format(RetailerGroupCacheKey, groupId), (key) =>
            {
                return _db.KvGroup.FirstOrDefaultAsync(r => r.Id == groupId);
            });
        }
    }
}
