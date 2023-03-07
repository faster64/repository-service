using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents
{
    public class UpdateEmployeeDeviceIntegrationEvent : IntegrationEvent
    {
        public int BranchId { get; set; }
        public long UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string IdentityKeyClocking { get; set; }

        [JsonConstructor]
        public UpdateEmployeeDeviceIntegrationEvent()
        {
        }

        public UpdateEmployeeDeviceIntegrationEvent(int branchId, long uId, string empCode, string idKeyCloking)
        {
            BranchId = branchId;
            UserId = uId;
            EmployeeCode = empCode;
            IdentityKeyClocking = idKeyCloking;
        }
    }
}