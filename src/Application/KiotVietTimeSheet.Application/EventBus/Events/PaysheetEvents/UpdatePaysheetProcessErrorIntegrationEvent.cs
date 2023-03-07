using System;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents
{
    public class UpdatePaysheetProcessErrorIntegrationEvent : IntegrationEvent
    {
        public string PaysheetCode { get; set; }
        public string PaysheetName { get; set; }
        public byte PaysheetStatus { get; set; }
        public DateTime? PaysheetCreatedDate { get; set; }
        public string Message { get; set; }
        public bool IsUpdate { get; set; }
        public PaysheetDto PaysheetDto { get; set; }
        public UpdatePaysheetProcessErrorIntegrationEvent(string paysheetCode, string paysheetName, byte paysheetStatus, string message, PaysheetDto paysheetDto, bool isUpdate)
        {
            PaysheetCode = paysheetCode;
            PaysheetName = paysheetName;
            PaysheetStatus = paysheetStatus;
            Message = message;
            PaysheetDto = paysheetDto;
            IsUpdate = isUpdate;
        }

        [JsonConstructor]
        public UpdatePaysheetProcessErrorIntegrationEvent()
        {

        }
    }
}
