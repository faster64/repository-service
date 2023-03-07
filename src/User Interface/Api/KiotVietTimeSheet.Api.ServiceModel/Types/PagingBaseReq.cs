namespace KiotVietTimeSheet.Api.ServiceModel.Types
{
    public abstract class PagingBaseReq
    {
        public virtual int? Skip { get; set; }
        public virtual int? Take { get; set; }
        public virtual bool? WithDeleted { get; set; }
        public virtual string[] OrderBy { get; set; }
        public virtual string[] OrderByDesc { get; set; }
    }
}
