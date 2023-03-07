using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using ServiceStack.Data;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Repositories
{
    public class CommissionObj
    {
        public long CommissionId { get; set; }
        public string CommissionName { get; set; }
        public bool CommissionIsActive { get; protected set; }
        public int CommissionTenantId { get; set; }
        public long CommissionCreatedBy { get; set; }
        public DateTime CommissionCreatedDate { get; set; }
        public long? CommissionModifiedBy { get; set; }
        public DateTime? CommissionModifiedDate { get; set; }
        public bool CommissionIsDeleted { get; set; }
        public long? CommissionDeletedBy { get; set; }
        public DateTime? CommissionDeletedDate { get; set; }
        public bool CommissionIsAllBranch { get; set; }
        public long CbId { get; set; }
        public int CbBranchId { get; set; }
    }

    public class CommissionReadOnlyRepository : OrmLiteRepository<Commission, long>, ICommissionReadOnlyRepository
    {
        public CommissionReadOnlyRepository(IDbConnectionFactory db, IAuthService authService)
            : base(db, authService)
        {

        }

        public async Task<PagingDataSource<CommissionDto>> FiltersAsync(ISqlExpression query, string[] includes, bool includeSoftDelete = false) => await LoadSelectDataSourceAsync<CommissionDto>(query, includes, includeSoftDelete);

        public async Task<List<Commission>> GetListForCurrentBranch(CommissionQueryFilter filter)
        {

            ISpecification<Commission> specification = new DefaultTrueSpec<Commission>();

            specification = specification.And(new FindCommissionByTenantIdSpec(AuthService.Context.TenantId));


            var queryCommissionIds = Db.From<CommissionBranch>(Db.TableAlias(typeof(CommissionBranch).Name))
                .And<CommissionBranch>(x => filter.BranchIds.Contains(x.BranchId))
                .SelectDistinct(x => x.CommissionId);
            var commissionIds = (await Db.SelectAsync<long>(queryCommissionIds)).Select(x => long.Parse(x.SqlValue())).ToList();


            var query = Db.From<Commission>(Db.TableAlias(typeof(Commission).Name))
                .Where(x => ((string.IsNullOrEmpty(filter.Keyword) || x.Name.Contains(filter.Keyword)) &&
                             (!x.IsDeleted || filter.IncludeIsDeleted) && (x.IsActive || filter.IncludeInActive) &&
                             (x.IsAllBranch || commissionIds.Contains(x.Id))) ||
                            (filter.IncludeCommissionIds != null && filter.IncludeCommissionIds.Contains(x.Id)))
                .Where<Commission>(specification.GetExpression());
            var result = (await Db.SelectAsync<object>(query)).ConvertTo<List<Commission>>();

            return result;
        }

        public async Task<List<Commission>> GetAllCommission()
        {

            var query = Db.From<Commission>()
                .LeftJoin<CommissionBranch>(
                    (commission, commissionBranch) => commission.Id == commissionBranch.CommissionId)
                .Where(c => c.TenantId == AuthService.Context.TenantId && !c.IsDeleted )
                .Select<Commission, CommissionBranch>((c, cb) => new
                {
                    CommissionId = c.Id,
                    CommissionName = c.Name,
                    CommissionIsDeleted = c.IsDeleted,
                    CommissionCreatedBy = c.CreatedBy,
                    CommissionCreatedDate = c.CreatedDate,
                    CommissionDeletedBy = c.DeletedBy,
                    CommissionDeletedDate = c.DeletedDate,
                    CommissionIsAllBranch = c.IsAllBranch,
                    CommissionModifiedBy = c.ModifiedBy,
                    CommissionModifiedDate = c.ModifiedDate,
                    CommissionTenantId = c.TenantId,
                    CbId = cb.Id,
                    CbBranchId = cb.BranchId
                });

            var queryResult = await Db.SelectAsync<CommissionObj>(query);
            return queryResult
                .GroupBy(cb => new
                {
                    cb.CommissionId,
                    cb.CommissionName,
                    cb.CommissionIsDeleted,
                    cb.CommissionCreatedBy,
                    cb.CommissionCreatedDate,
                    cb.CommissionDeletedBy,
                    cb.CommissionDeletedDate,
                    cb.CommissionIsAllBranch,
                    cb.CommissionModifiedBy,
                    cb.CommissionModifiedDate,
                    cb.CommissionTenantId
                })
                .Select(group => new Commission
                {
                    Id = group.Key.CommissionId,
                    Name = group.Key.CommissionName,
                    IsDeleted = group.Key.CommissionIsDeleted,
                    CreatedBy = group.Key.CommissionCreatedBy,
                    CreatedDate = group.Key.CommissionCreatedDate,
                    DeletedBy = group.Key.CommissionDeletedBy,
                    DeletedDate = group.Key.CommissionDeletedDate,
                    IsAllBranch = group.Key.CommissionIsAllBranch,
                    ModifiedBy = group.Key.CommissionModifiedBy,
                    ModifiedDate = group.Key.CommissionModifiedDate,
                    TenantId = group.Key.CommissionTenantId,
                    CommissionBranches = group.Select(g => new CommissionBranch
                    {
                        Id = g.CbId,
                        TenantId = g.CommissionTenantId,
                        BranchId = g.CbBranchId,
                        CommissionId = g.CommissionId
                    }).ToList()
                })
                .ToList();
        }
    }
}
