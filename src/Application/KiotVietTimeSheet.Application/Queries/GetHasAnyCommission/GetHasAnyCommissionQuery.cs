using KiotVietTimeSheet.Application.Abstractions;

namespace KiotVietTimeSheet.Application.Queries.GetHasAnyCommission
{
    public class GetHasAnyCommissionQuery : QueryBase<bool>
    {
        public bool IncludeDeleted { get; set; }

        public GetHasAnyCommissionQuery(bool includeDeleted = false)
        {
            IncludeDeleted = includeDeleted;
        }
    }
}
