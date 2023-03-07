
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;
using Newtonsoft.Json;
using KiotVietTimeSheet.SharedKernel.EventBus;
using System.ComponentModel.DataAnnotations.Schema;
using KiotVietTimeSheet.Utilities;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models
{
    [Table("IntegrationEventLogEntry")]
    public class IntegrationEventLogEntry2 : BaseEntity,
        IAggregateRoot
    {
        public IntegrationEventLogEntry2(IntegrationEvent theEvent, Guid transactionId)
        {
            EventId = theEvent.Id;
            EventType = theEvent.GetType().Name;
            CreatedTime = theEvent.CreatedTime;
            State = 1;
            Data = JsonConvert.SerializeObject(theEvent, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            TimeSent = 0;
            CreatedTime = DateTime.Now;
            TransactionId = transactionId;
        }

        // EF Constructor
        private IntegrationEventLogEntry2() { }

        [Key]
        public Guid EventId { get; set; }

        public Guid TransactionId { get; set; }

        public string EventType { get; set; }

        public string Data { get; set; }

        public DateTime CreatedTime { get; set; }

        public int State { get; set; }

        public DateTime? ProcessedTime { get; set; }

        public int TimeSent { get; set; }

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        public IntegrationEventLogEntry2 DeserializeJsonContent(Type type)
        {
            //Parse json class contain protect property
            IntegrationEvent = JsonConvert.DeserializeObject(Data, type, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            }) as IntegrationEvent;
            return this;
        }

    }
}
