using System.Threading.Tasks;
using KiotVietTimeSheet.Infrastructure.DbMaster.Models;

namespace KiotVietTimeSheet.Infrastructure.DbMaster
{
    public interface IMasterDbService
    {
        Task<KvRetailer> GetRetailerAsync(int retailerId);
        Task<KvGroup> GetGroupAsync(int groupId);
    }
}
