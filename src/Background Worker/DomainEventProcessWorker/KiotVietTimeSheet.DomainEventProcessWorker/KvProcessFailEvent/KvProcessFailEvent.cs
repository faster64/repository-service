using System;
using System.ComponentModel.DataAnnotations.Schema;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent
{
    public enum KvProcessFailEventStates
    {
        Pending = 1,
        InProgress = 2,
        Resolved = 3
    }

    public class KvProcessFailEvent
    {
        public long Id { get; set; }
        public Guid EventId { get; set; }
        public string EventData { get; set; }
        public string EventType { get; set; }
        public string ErrorMessage { get; set; }
        public byte State { get; set; }
        public int RetryTimes { get; set; }
        public DateTime? ProcessedTime { get; set; }
        public System.DateTime CreatedTime { get; set; }

        public KvProcessFailEvent(IntegrationEvent theEvent, string errorMessage)
        {
            EventId = theEvent.Id;
            EventType = theEvent.GetType().Name;
            State = (byte)KvProcessFailEventStates.Pending;
            EventData = JsonConvert.SerializeObject(theEvent, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            ErrorMessage = errorMessage;
            RetryTimes = 0;
            CreatedTime = DateTime.Now;
        }

        // Ef
        public KvProcessFailEvent()
        {

        }

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        public KvProcessFailEvent DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(EventData, type) as IntegrationEvent;
            return this;
        }

        public void UpdateErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
