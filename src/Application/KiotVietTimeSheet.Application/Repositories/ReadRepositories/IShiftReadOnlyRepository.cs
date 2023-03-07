using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IShiftReadOnlyRepository : IBaseReadOnlyRepository<Shift, long>
    {
        Task<PagingDataSource<ShiftDto>> FiltersAsync(ISqlExpression query);


        /// <summary>
        /// Lấy danh sách ca theo chi nhánh làm việc
        /// </summary>
        /// <param name="branchIds"></param>
        /// <param name="shiftIds"></param>
        /// <param name="includeShiftIds"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<List<Shift>> GetShiftMultipleBranchOrderByFromAndTo(List<int> branchIds, List<long> shiftIds,
            List<long> includeShiftIds = null, bool includeDeleted = false);

        Task<Shift> GetShiftByIdForClockingGps(long id, bool includeDeleted = true);

        Task<List<Shift>> GetShiftBySpecificationForClockingGpsAsync(ISpecification<Shift> spec, bool includeDeleted = false);
    }
}
