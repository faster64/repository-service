namespace KiotVietTimeSheet.SharedKernel.Interfaces
{
    public interface ISoftDelete : IDeletedBy, IDeletedDate
    {
        bool IsDeleted { get; set; }
    }
    public interface ISoftDeleteV2
    {
        byte Status { get; set; }
    }
}
