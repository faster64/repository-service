using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetShiftOrderByFromAndTo
{
    public sealed class GetShiftOrderByFromAndToQuery : QueryBase<List<ShiftDto>>
    {
        public int BranchId { get; set; }
        public List<long> ShiftIds { get; set; }
        public string Keyword { get; set; }

        public GetShiftOrderByFromAndToQuery(int branchId, List<long> shiftIds, string keyword)
        {
            BranchId = branchId;
            ShiftIds = shiftIds;
            Keyword = keyword;
        }
    }
}
