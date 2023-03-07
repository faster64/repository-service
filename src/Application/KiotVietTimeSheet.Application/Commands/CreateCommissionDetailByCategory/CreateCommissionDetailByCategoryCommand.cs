using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;

namespace KiotVietTimeSheet.Application.Commands.CreateCommissionDetailByCategory
{
    public class CreateCommissionDetailByCategoryCommand : BaseCommand
    {
        public List<CommissionDetail> CommissionDetails { get; set; }
        public CreateCommissionDetailByCategoryCommand(
            List<CommissionDetail> commissionDetails)
        {
            CommissionDetails = commissionDetails;
        }
    }
}
