using System;
using System.ComponentModel.DataAnnotations.Schema;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Models
{
    public enum AuditProcessFailEventStates
    {
        Pending = 1,
        InProgress = 2,
        Resolved = 3
    }

    public class AuditProcessFailEvent
    {
        public long Id { get; set; }

        public Guid EventId { get; set; }

        public string EventData { get; set; }

        public string EventType { get; set; }

        public string ErrorMessage { get; set; }

        public AuditProcessFailEventStates State { get; set; }

        public int RetryTimes { get; set; }

        public DateTime? ProcessedTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public void UpdateErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public AuditProcessFailEvent(IntegrationEvent theEvent, string errorMessage)
        {
            EventId = theEvent.Id;
            EventType = theEvent.GetType().Name;
            State = AuditProcessFailEventStates.Pending;
            EventData = JsonConvert.SerializeObject(theEvent, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            ErrorMessage = errorMessage;
            RetryTimes = 0;
            CreatedTime = DateTime.Now;
        }

        // Ef
        public AuditProcessFailEvent()
        {

        }

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        public AuditProcessFailEvent DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(EventData, type) as IntegrationEvent;
            return this;
        }
    }
}
