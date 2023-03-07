using System;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        Task CommitAsync(bool dispatchEvent = true);

        Task CommitBySaveChangesAsync(bool dispatchEvent = true);
    }
}
