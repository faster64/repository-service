using KiotVietTimeSheet.SharedKernel.Models;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.SharedKernel.Domain
{
    public interface IEventDispatcher
    {
        Task FireEvent<T>(T @event) where T : Message;
    }
}
