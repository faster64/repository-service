using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite
{
    public class OrmLiteRepository<TEntity, TPrimaryKey> : IBaseReadOnlyRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region Properties
        private readonly IDbConnectionFactory _dbFactory;
        private IDbConnection _db;
        protected IAuthService AuthService;
        protected IDbConnection Db => _db ?? (_db = _dbFactory.OpenDbConnection());
        #endregion

        #region Constructor
        protected OrmLiteRepository(IDbConnectionFactory dbFactory, IAuthService authService)
        {
            _dbFactory = dbFactory;
            AuthService = authService;

            OrmLiteConfig.InsertFilter = (dbCmd, row) => SetRowInfor(authService, row);

            OrmLiteConfig.UpdateFilter = (dbCmd, row) => SetModifiedByAndDate(authService, row);
        }
        #endregion

        #region Find And Get Methods
        public virtual async Task<TEntity> FindAndIncludeBySpecificationAsync(ISpecification<TEntity> spec, string[] includes, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            if (includes != null && includes.Any())
            {
                var result = await Db.LoadSelectAsync(spec.GetExpression(), includes);
                return result.FirstOrDefault();
            }

            return await Db.SingleAsync(spec.GetExpression());
        }

        public virtual async Task<TEntity> FindBySpecificationAsync(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            if (!reference) return await Db.SingleAsync(spec.GetExpression());
            var result = await Db.LoadSelectAsync(spec.GetExpression());
            return result.FirstOrDefault();
        }

        public virtual async Task<TEntity> FindBySpecificationWithIncludeAsync(ISpecification<TEntity> spec, string[] include = null, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            var result = await Db.LoadSelectAsync(spec.GetExpression(), include);
            return result.FirstOrDefault();
        }

        public virtual async Task<TEntity> FindByIdAsync(object id, bool reference = false, bool includeSoftDelete = false)
        {
            return await FindBySpecificationAsync(GetFindByPrimaryKeySpec((long)id), reference, includeSoftDelete);
        }

        public async Task<List<TEntity>> GetBySpecification(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            return reference ? Db.LoadSelect(spec.GetExpression()) : Db.Select(spec.GetExpression());
        }

        public virtual async Task<List<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            if (reference) return await Db.LoadSelectAsync(spec.GetExpression());
            return await Db.SelectAsync(spec.GetExpression());
        }

        public virtual async Task<List<TEntity>> FindByIdsAsync(object[] ids, bool reference = false, bool includeSoftDelete = false)
        {
            return await GetBySpecificationAsync(GetFindByMultiplePrimaryKeySpec(ids), reference, includeSoftDelete);
        }

        public List<TEntity> GetAllByTenant()
        {
            var spec = GuardDataAccessByTenant(new DefaultTrueSpec<TEntity>());
            return Db.Select(spec.GetExpression());
        }

        public async Task<List<TEntity>> GetAllAsync(bool reference = false, bool includeSoftDelete = false)
        {
            return await GetBySpecificationAsync(new DefaultTrueSpec<TEntity>(), reference, includeSoftDelete);
        }
        public async Task<List<TEntity>> GetAllWithinIncludeAsync(string[] include = null, bool includeSoftDelete = false)
        {
            return await FindListBySpecificationWithIncludeAsync(new DefaultTrueSpec<TEntity>(), include, includeSoftDelete);
        }

        public virtual async Task<List<TEntity>> FindListBySpecificationWithIncludeAsync(ISpecification<TEntity> spec, string[] include = null, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            var result = await Db.LoadSelectAsync(spec.GetExpression(), include);
            return result.ToList();
        }

        public async Task<PagingDataSource<TInto>> LoadSelectDataSourceAsync<TInto>(ISqlExpression query, string[] include = null, bool includeSoftDelete = false) where TInto : class
        {
            var q = (SqlExpression<TEntity>)query;
            q.Where((await GuardDataAccess(new DefaultTrueSpec<TEntity>(), includeSoftDelete)).GetExpression());

            return new PagingDataSource<TInto>
            {
                Total = (int)await Db.CountAsync(q),
                Data = await Db.LoadSelectAsync<TInto, TEntity>(q, include)
            };
        }

        public virtual async Task<int> SingleAsync(ISqlExpression query)
        {
            var q = (SqlExpression<TEntity>)query;
            q.Where((await GuardDataAccess(new DefaultTrueSpec<TEntity>())).GetExpression());
            return await Db.SingleAsync<int>(q);
        }

        public async Task<List<TInto>> SelectIntoBySpecificationAsync<TInto>(ISpecification<TEntity> spec)
        {
            spec = await GuardDataAccess(spec);
            var q = Db.From<TEntity>().Where(spec.GetExpression());
            return await Db.SelectAsync<TInto>(q);
        }

        public async Task<long> CountByExpression(Expression<Func<TEntity, bool>> filter)
        {
            return await Db.CountAsync(filter);
        }
        public async Task<long> CountBySpecificationAsync(ISpecification<TEntity> spec)
        {
            spec = await GuardDataAccess(spec);
            return await Db.CountAsync(spec.GetExpression());
        }

        public async Task<bool> AnyBySpecificationAsync(ISpecification<TEntity> spec, bool includeSoftDelete = false)
        {
            spec = await GuardDataAccess(spec, includeSoftDelete);
            return await Db.ExistsAsync(spec.GetExpression());
        }

        public async Task<ISpecification<TEntity>> GuardDataAccess(ISpecification<TEntity> spec, bool includeSoftDelete = false)
        {
            var type = typeof(TEntity);

            var limitByTenantId = new LimitByTenantIdSpec<TEntity>(AuthService.Context.TenantId);
            var authorizedBranchIds = await AuthService.GetAuthorizedBranchIds();
            var filterSoftDelete = new FilterSoftDeleteSpec<TEntity>();

            // Limit by tenant
            if (typeof(TEntity).HasInterface(typeof(ITenantId)))
                spec = spec.And(limitByTenantId);

            //// Limit by branch access
            if (typeof(TEntity).HasInterface(typeof(IBranchId)) && !AuthService.Context.User.IsAdmin && (type != typeof(BranchSetting) || !AuthService.HasPermittedOnBranch(AuthService.Context.BranchId, authorizedBranchIds)))
            {
                var limitByBranchAccess = new LimitByBranchAccessSpec<TEntity>(authorizedBranchIds.ToList());
                // #499 làm như hệ thống cũ
                spec = spec.And(limitByBranchAccess);
            }

            // Soft delete
            if (!AuthService.WithDeleted && typeof(TEntity).HasInterface(typeof(ISoftDelete)) && !includeSoftDelete)
            {
                spec = spec.And(filterSoftDelete);
            }

            return spec;
        }

        /// Hàm này chỉ phục vụ cho việc lọc dữ liệu theo Tenant, không lọc theo branch.
        /// Hiện tại mới chỉ phục vụ cho nghiệp vụ của máy chấm công.
        public ISpecification<TEntity> GuardDataAccessByTenant(ISpecification<TEntity> spec, bool includeSoftDelete = false)
        {
            _ = typeof(TEntity);

            var limitByTenantId = new LimitByTenantIdSpec<TEntity>(AuthService.Context.TenantId);
            var filterSoftDelete = new FilterSoftDeleteSpec<TEntity>();

            // Limit by tenant
            if (typeof(TEntity).HasInterface(typeof(ITenantId)))
                spec = spec.And(limitByTenantId);

            // Soft delete
            if (!AuthService.WithDeleted && typeof(TEntity).HasInterface(typeof(ISoftDelete)) && !includeSoftDelete)
            {
                spec = spec.And(filterSoftDelete);
            }

            return spec;
        }
        #endregion

        public void DisabledSoftDeleteFilter()
        {
            AuthService.WithDeleted = true;
        }

        #region Private Methods

        private ISpecification<TEntity> GetFindByMultiplePrimaryKeySpec(object[] keys)
        {
            if (typeof(TEntity).HasInterface(typeof(IEntityIdlong)))
            {
                return new FindByMultipleEntityIdLongSpec<TEntity>(keys);
            }

            var exErr = new Exception("Can't find spec matched primary key");
            throw exErr;
        }

        private ISpecification<TEntity> GetFindByPrimaryKeySpec(long key)
        {
            if (typeof(TEntity).HasInterface(typeof(IEntityIdlong)))
            {
                return new FindByEntityIdLongSpec<TEntity>(key);
            }

            var exErr = new Exception("Can't find spec matched primary key");
            throw exErr;
        }

        private void SetRowInfor(IAuthService authService, object row)
        {
            if (row is ICreatedDate auditRow && auditRow.CreatedDate == default(DateTime))
                auditRow.CreatedDate = DateTime.Now;

            if (row is ITenantId tenantEntity && tenantEntity.TenantId == 0)
                tenantEntity.TenantId = AuthService.Context.TenantId;

            if (row is IBranchId branchEntity && branchEntity.BranchId == 0)
                branchEntity.BranchId = AuthService.Context.BranchId;

            if (row is ICreatedBy createdByEntity && createdByEntity.CreatedBy == 0)
                createdByEntity.CreatedBy = authService.Context.User.Id;
        }

        private static void SetModifiedByAndDate(IAuthService authService, object row)
        {
            var auditRow = (IModifiedDate)row;
            if (auditRow != null) auditRow.ModifiedDate = DateTime.Now;

            if (row is IModifiedBy modifyByRow && modifyByRow.ModifiedBy == 0)
                modifyByRow.ModifiedBy = authService.Context.User.Id;
        }
        #endregion

        #region Protected Methods

        protected SqlExpression<TEntity> FromQuery()
        {
            return Db.From<TEntity>();
        }

        protected SqlExpression<TEntity> BuildPagingFilterSqlExpression(IQueryFilter filter)
        {
            var query = Db.From<TEntity>(Db.TableAlias(typeof(TEntity).Name));

            if (filter.OrderBy != null && filter.OrderBy.Length > 0)
            {
                query = query.OrderByFields(filter.OrderBy);
            }
            else if (filter.OrderByDesc != null && filter.OrderByDesc.Length > 0)
                query = query.OrderByFieldsDescending(filter.OrderByDesc);
            else
                query = query.OrderByFieldsDescending("Id");

            query = query.Skip(filter.Skip)
                .Take(filter.Take);
            return query;
        }

        #endregion

        private bool _disposedValue = false; // To detect redundant calls

        protected void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                // TODOs: dispose managed state (managed objects).
                _db?.Close();
                _db?.Dispose();
            }

            // TODOs: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODOs: set large fields to null.

            _disposedValue = true;
        }

        // TODOs: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // Dispose ~OrmLiteRepository 
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //  call  Dispose false
        // 

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODOs: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
    }
}
