using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class ShiftReadOnlyRepository : OrmLiteRepository<Shift, long>, IShiftReadOnlyRepository
    {
        private readonly IAuthService _authService;
        public ShiftReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {
            _authService = authService;
        }

        public virtual async Task<PagingDataSource<ShiftDto>> FiltersAsync(ISqlExpression query)
        {
            return await LoadSelectDataSourceAsync<ShiftDto>(query);
        }

        /// <inheritdoc />
        public async Task<List<Shift>> GetShiftMultipleBranchOrderByFromAndTo(List<int> branchIds, List<long> shiftIds,
            List<long> includeShiftIds = null, bool includeDeleted = false)
        {
            var specification = new LimitByTenantIdSpec<Shift>(_authService.Context.TenantId).And(
                new FindShiftByBranchIdsSpec(branchIds));
            if (shiftIds != null && shiftIds.Any())
            {
                specification = specification.And(new FindShiftByShiftIdsSpec(shiftIds));
            }

            if (includeShiftIds != null && includeShiftIds.Any())
            {
                specification = specification.Or(new FindShiftByShiftIdsSpec(includeShiftIds));
            }

            var query = Db.From<Shift>()
                .Where(s => includeDeleted || s.IsDeleted == includeDeleted)
                .Where<Shift>(specification.GetExpression()).Select();

            return await Db.LoadSelectAsync(query);
        }

        public async Task<Shift> GetShiftByIdForClockingGps(long id, bool includeDeleted = true)
        {
            var query = Db.From<Shift>()
                .Where(s => s.TenantId == _authService.Context.TenantId)
                .Where(s => includeDeleted || s.IsDeleted == includeDeleted)
                .Where(s => s.Id == id);

            var shift = await Db.SelectAsync(query);
            return shift.FirstOrDefault();
        }

        public async Task<List<Shift>> GetShiftBySpecificationForClockingGpsAsync(ISpecification<Shift> spec, bool includeDeleted = false)
        {
            var filterSoftDelete = new FilterSoftDeleteSpec<Shift>();

            // Soft delete
            if (!_authService.WithDeleted && typeof(Shift).HasInterface(typeof(ISoftDelete)) && !includeDeleted)
            {
                spec = spec.And(filterSoftDelete);
            }

            return await Db.SelectAsync(spec.GetExpression());
        }
    }
}
