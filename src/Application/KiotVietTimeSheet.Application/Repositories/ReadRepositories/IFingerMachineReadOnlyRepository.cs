using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IFingerMachineReadOnlyRepository : IBaseReadOnlyRepository<FingerMachine, long>
    {
        Task<PagingDataSource<FingerMachine>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false);

        Task<bool> MachineNameIsAlreadyExistsInBranchAsync(FingerMachine machine);
        Task<bool> MachineIdIsAlreadyExistsInBranchAsync(FingerMachine machine);
        Task<FingerMachine> GetFingerMachineByMachineId(string machineId);

    }
}
