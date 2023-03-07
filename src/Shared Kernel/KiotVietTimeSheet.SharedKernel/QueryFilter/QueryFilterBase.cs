using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.SharedKernel.QueryFilter
{
    public abstract class QueryFilterBase<T> : IQueryFilter
        where T : BaseEntity
    {
        protected QueryFilterBase() { }

        public virtual int? Skip { get; set; }
        public virtual int? Take { get; set; }
        public virtual bool? WithDeleted { get; set; }
        public virtual string[] OrderBy { get; set; }
        public virtual string[] OrderByDesc { get; set; }
        public virtual string[] Includes { get; set; }
    }
}
