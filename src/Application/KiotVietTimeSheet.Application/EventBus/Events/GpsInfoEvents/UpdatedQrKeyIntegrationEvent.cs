using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents
{
    public class UpdatedQrKeyIntegrationEvent : IntegrationEvent
    {
        public GpsInfo GpsInfo { get; set; }
        public string BranchName { get; set; }

        [JsonConstructor]
        public UpdatedQrKeyIntegrationEvent(GpsInfo gpsInfo, string branchName)
        {
            GpsInfo = gpsInfo;
            BranchName = branchName;
        }
    }
}