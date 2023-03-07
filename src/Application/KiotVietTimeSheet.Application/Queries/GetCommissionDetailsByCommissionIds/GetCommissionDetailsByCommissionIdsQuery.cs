using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByCommissionIds
{
    public class GetCommissionDetailsByCommissionIdsQuery : QueryBase<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public List<long> ProductIds { get; set; }

        public GetCommissionDetailsByCommissionIdsQuery(List<long> commissionIds, List<long> productIds)
        {
            CommissionIds = commissionIds;
            ProductIds = productIds;
        }
    }
}
