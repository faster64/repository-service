using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents
{
    public class CancelPayslipIntegrationEvent : IntegrationEvent
    {
        public Payslip Payslip { get; set; }
        public CancelPayslipIntegrationEvent(CancelPayslipEvent @event)
        {
            Payslip = @event.Payslip;
        }

        [JsonConstructor]
        public CancelPayslipIntegrationEvent()
        {

        }
    }
}
