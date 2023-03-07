using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetListGpsInfo
{
    public class GetGpsInfoForClockingGpsQuery : QueryBase<GpsInfoDto>
    {
        public int BranchId { get; set; }
        public GetGpsInfoForClockingGpsQuery(int branchId)
        {
            BranchId = branchId;
        }

    }
}
