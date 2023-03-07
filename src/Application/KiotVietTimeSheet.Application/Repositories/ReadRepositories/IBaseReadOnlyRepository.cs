using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IBaseReadOnlyRepository<TEntity, TPrimaryKey> : IDisposable
    {
        List<TEntity> GetAllByTenant();

        Task<List<TEntity>> GetAllAsync(bool reference = false, bool includeSoftDelete = false);
        Task<List<TEntity>> GetAllWithinIncludeAsync(string[] include = null, bool includeSoftDelete = false);

        Task<List<TEntity>> GetBySpecification(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false);
        Task<TEntity> FindByIdAsync(object id, bool reference = false, bool includeSoftDelete = false);

        Task<List<TEntity>> FindByIdsAsync(object[] ids, bool reference = false, bool includeSoftDelete = false);

        Task<TEntity> FindBySpecificationAsync(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false);

        Task<TEntity> FindBySpecificationWithIncludeAsync(ISpecification<TEntity> spec, string[] include = null, bool includeSoftDelete = false);

        Task<TEntity> FindAndIncludeBySpecificationAsync(ISpecification<TEntity> spec, string[] includes, bool includeSoftDelete = false);

        Task<List<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec, bool reference = false, bool includeSoftDelete = false);

        Task<PagingDataSource<TInto>> LoadSelectDataSourceAsync<TInto>(ISqlExpression query, string[] include = null, bool includeSoftDelete = false) where TInto : class;

        Task<int> SingleAsync(ISqlExpression query);

        Task<List<TInto>> SelectIntoBySpecificationAsync<TInto>(ISpecification<TEntity> spec);

        Task<bool> AnyBySpecificationAsync(ISpecification<TEntity> spec, bool includeSoftDelete = false);

        Task<long> CountBySpecificationAsync(ISpecification<TEntity> spec);

        Task<long> CountByExpression(Expression<Func<TEntity, bool>> filter);

        void DisabledSoftDeleteFilter();

        Task<ISpecification<TEntity>> GuardDataAccess(ISpecification<TEntity> spec, bool includeSoftDelete = false);
    }
}
