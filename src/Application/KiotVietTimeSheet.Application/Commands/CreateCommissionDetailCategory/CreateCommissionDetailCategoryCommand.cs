using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailCategory
{
    public class CreateCommissionDetailCategoryCommand : BaseCommand<List<CommissionDetailDto>>
    {
        public List<long> CommissionIds { get; set; }
        public List<int> CategoryIds { get; set; }

        public CreateCommissionDetailCategoryCommand(List<long> commissionIds, List<int> categoryIds)
        {
            CommissionIds = commissionIds;
            CategoryIds = categoryIds;
        }
    }

}
