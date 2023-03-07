using KiotVietTimeSheet.Application.UnitOfWork;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IBaseWriteOnlyRepository<TEntity> where TEntity : class
    {
        IUnitOfWork UnitOfWork { get; }
        #region Get Methods

        Task<TEntity> FindByIdAsync(object id);
        Task<TEntity> FindBySpecificationAsync(ISpecification<TEntity> spec, string include = null);
        Task<TEntity> FindBySpecificationWithIncludesAsync(ISpecification<TEntity> spec, List<string> includes = null);
        Task<List<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec, string include = null);
        Task<long> CountByExpressionAsync(Expression<Func<TEntity, bool>> filter);
        Task<long> CountBySpecificationAsync(ISpecification<TEntity> spec);
        Task<bool> AnyBySpecificationAsync(ISpecification<TEntity> spec);

        #endregion

        #region Add Methods
        TEntity Add(TEntity entity, string[] excludeProperties = null);
        TEntity AddEntityNotGuard(TEntity entity);
        List<TEntity> BatchAdd(List<TEntity> entities, string[] excludeProperties = null);
        List<TEntity> BatchAddNotGuard(List<TEntity> entities);
        #endregion

        #region Update Methods
        void Update(TEntity entity, string[] excludeProperties = null);
        void UpdateEntityNotGuard(TEntity entity);
        void BatchUpdate(List<TEntity> entities, string[] excludeProperties = null);
        #endregion

        #region Delete Methods
        void Delete(TEntity entity);
        void BatchDelete(List<TEntity> entities);
        #endregion

        #region Detach methods
        void Detach(TEntity entity);
        void BatchDetach(List<TEntity> entities);
        TEntity DetachByClone(TEntity entity, string[] includes = null);
        IList<TEntity> BatchDetachByClone(IList<TEntity> entities, string[] includes = null);
        #endregion

        #region Util Methods
        void DisabledSoftDeleteFilter();
        void GenerateRangeNextCode(List<TEntity> entities);
        #endregion
    }
}
