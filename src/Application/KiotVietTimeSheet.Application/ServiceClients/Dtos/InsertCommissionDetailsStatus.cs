using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;

namespace KiotVietTimeSheet.Application.ServiceClients.Dtos
{
    public class InsertCommissionDetailsStatus
    {
        public InsertCommissionDetailStatusEnums Status { get; set; }
        public string Message { get; set; }
    }
}
