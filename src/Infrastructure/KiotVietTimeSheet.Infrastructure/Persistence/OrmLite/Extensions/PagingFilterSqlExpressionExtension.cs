using KiotVietTimeSheet.SharedKernel.QueryFilter;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.Extensions
{
    public static class PagingFilterSqlExpressionExtension
    {
        public static SqlExpression<T> PagingFilter<T>(this SqlExpression<T> query, IQueryFilter filter)
        {
            if (filter.OrderBy != null && filter.OrderBy.Length > 0)
                query = query.OrderByFields(filter.OrderBy);
            else if (filter.OrderByDesc != null && filter.OrderByDesc.Length > 0)
                query = query.OrderByFieldsDescending(filter.OrderByDesc);
            else
                query = query.OrderBy();
            query.Limit(filter.Skip, filter.Take);
            return query;
        }
    }
}
