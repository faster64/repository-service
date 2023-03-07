using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByProductCategoryDataTrial
{
    public class CreateCommissionDetailByProductCategoryDataTrialCommand : BaseCommand<List<CommissionDetailDto>>, IInternalRequest
    {
        public List<long> CommissionIds { get; set; }
        public ProductCategoryReqDto ProductCategory { get; set; }
        public long UserIdAdmin { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }
        public CreateCommissionDetailByProductCategoryDataTrialCommand(List<long> commissionIds,
            ProductCategoryReqDto productCategory, long userIdAdmin, int tenantId, int groupId)
        {
            CommissionIds = commissionIds;
            ProductCategory = productCategory;
            UserIdAdmin = userIdAdmin;
            TenantId = tenantId;
            GroupId = groupId;
        }
    }
}
