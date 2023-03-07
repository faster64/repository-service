using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetShiftByIds
{
    public sealed class GetShiftByIdsQuery : QueryBase<List<ShiftDto>>
    {
        public List<long> Ids { get; set; }

        public GetShiftByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
