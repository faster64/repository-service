using System;
using System.ComponentModel.DataAnnotations.Schema;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.Utilities;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Models
{
    public class IntegrationEventLogEntry
    {
        public IntegrationEventLogEntry(IntegrationEvent theEvent, Guid transactionId)
        {
            EventId = theEvent.Id;
            EventType = theEvent.GetType().Name;
            CreatedTime = theEvent.CreatedTime;
            State = IntegrationEventLogState.NotPublished;
            Data = JsonConvert.SerializeObject(theEvent, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            TimeSent = 0;
            CreatedTime = DateTime.Now;
            TransactionId = transactionId;
        }

        // EF Constructor
        private IntegrationEventLogEntry() { }

        public Guid EventId { get; set; }

        public Guid TransactionId { get; set; }

        public string EventType { get; set; }

        public string Data { get; set; }

        public DateTime CreatedTime { get; set; }

        public IntegrationEventLogState State { get; set; }

        public DateTime? ProcessedTime { get; set; }

        public int TimeSent { get; set; }

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            //Parse json class contain protect property
            IntegrationEvent = JsonConvert.DeserializeObject(Data, type, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            }) as IntegrationEvent;
            return this;
        }

        public enum IntegrationEventLogState
        {
            NotPublished = 1,
            InProgress = 2,
            Published = 3,
            PublishFailed = 4
        }
    }
}
