using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events
{
    public class SocketPaySheetCreateIntegrationEvent : IntegrationEvent
    {
        public int RetailerId { get; set; }

        public long PaySheetId { get; set; }

        public byte PaySheetStatus { get; set; }

        public string Description { get; set; }

        public string EventType { get; set; }

        public int StatusCode { get; set; }
        
        public int BranchId { get; set; }

        public SocketPaySheetCreateIntegrationEvent(
            int retailerId, int branchId, long paySheetId, 
            byte paySheetStatus, string description, 
            string eventType, int statusCode)
        {
            RetailerId = retailerId;
            BranchId = branchId;
            PaySheetId = paySheetId;
            PaySheetStatus = paySheetStatus;
            Description = description;
            EventType = eventType;
            StatusCode = statusCode;
        }
    }
}
