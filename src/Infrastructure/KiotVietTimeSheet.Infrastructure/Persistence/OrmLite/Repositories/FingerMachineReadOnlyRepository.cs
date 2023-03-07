using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerMachineAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class FingerMachineReadOnlyRepository : OrmLiteRepository<FingerMachine, long>, IFingerMachineReadOnlyRepository
    {
        public FingerMachineReadOnlyRepository(
            IDbConnectionFactory dbFactory,
            IAuthService authService) : base(dbFactory, authService)
        {
        }

        public async Task<PagingDataSource<FingerMachine>> FiltersAsync(ISqlExpression query, bool includeSoftDelete = false)
        {
            var fingerMachineDataSource = await LoadSelectDataSourceAsync<FingerMachine>(query, null, includeSoftDelete);
            var data = fingerMachineDataSource.Data.ToList();

            return new PagingDataSource<FingerMachine>
            {
                Total = fingerMachineDataSource.Total,
                Filters = fingerMachineDataSource.Filters,
                Data = data
            };
        }

        public async Task<bool> MachineIdIsAlreadyExistsInBranchAsync(FingerMachine machine)
        {
            var existMachineName = await AnyBySpecificationAsync(new FindFingerMachineByMachineId(machine.MachineId));
            return existMachineName;
        }

        public async Task<bool> MachineNameIsAlreadyExistsInBranchAsync(FingerMachine machine)
        {
            var existMachineName = await AnyBySpecificationAsync(
                new FindFingerMachineByBranchIdSpec(machine.BranchId)
                    .And(new FindFingerMachineByNameSpec(machine.MachineName))
                    .Not(new FindByEntityIdLongSpec<FingerMachine>(machine.Id)));

            return existMachineName;
        }

        public async Task<FingerMachine> GetFingerMachineByMachineId(string machineId)
        {
            var machine = await FindBySpecificationWithIncludeAsync(new FindFingerMachineByMachineIdSpec(machineId));
            return machine;
        }
    }
}
