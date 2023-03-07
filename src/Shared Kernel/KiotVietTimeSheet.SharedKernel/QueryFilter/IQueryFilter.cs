namespace KiotVietTimeSheet.SharedKernel.QueryFilter
{
    public interface IQueryFilter
    {
        int? Skip { get; set; }
        int? Take { get; set; }
        bool? WithDeleted { get; set; }
        string[] OrderBy { get; set; }
        string[] OrderByDesc { get; set; }
    }
}
