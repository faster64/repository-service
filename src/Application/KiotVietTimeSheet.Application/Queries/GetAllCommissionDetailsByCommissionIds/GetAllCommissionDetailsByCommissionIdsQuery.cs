using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetAllCommissionDetailsByCommissionIds
{
    public class GetAllCommissionDetailsByCommissionIdsQuery : QueryBase<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public GetAllCommissionDetailsByCommissionIdsQuery(List<long> commissionIds)
        {
            CommissionIds = commissionIds;
        }
    }
}
