using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class GetProductByCategoryIdReq
    {
        public int CategoryId { get; set; }
        public int RetailerId { get; set; }
        public List<CommissionDetailDto> CommissionDetails { get; set; }
        public int BranchId { get; set; }
        public long UserId { get; set; }
        public int GroupId { get; set; }
        public string RetailerCode { get; set; }
    }
}
