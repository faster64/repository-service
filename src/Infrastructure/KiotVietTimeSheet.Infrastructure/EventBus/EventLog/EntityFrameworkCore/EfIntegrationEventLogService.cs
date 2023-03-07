using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore
{

    public class EfIntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly EfIntegrationEventLogContext _efIntegrationEventLogContext;
        private readonly List<Type> _eventTypes;
        private ILogger<EfIntegrationEventLogService> _logger;
        private ILogger<EfIntegrationEventLogService> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<EfIntegrationEventLogService>>());
        public EfIntegrationEventLogService(DbConnection dbConnection, List<Type> eventTypes = null)
        {
            _efIntegrationEventLogContext = new EfIntegrationEventLogContext(
                new DbContextOptionsBuilder<EfIntegrationEventLogContext>()
                    .UseSqlServer(dbConnection)
                    .Options
            );

            if (eventTypes == null)
            {
                _eventTypes = Assembly.Load(Assembly.GetEntryAssembly()?.FullName ?? throw new InvalidOperationException())
                    .GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType == typeof(IntegrationEvent))
                    .ToList();
            }
            else
            {
                _eventTypes = eventTypes;
            }
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> GetPublishableEventAsync(Guid transactionId)
        {
            return await _efIntegrationEventLogContext.IntegrationEventLogEntries
                .Where(e => e.TransactionId == transactionId &&
                            e.State == IntegrationEventLogEntry.IntegrationEventLogState.NotPublished)
                .OrderBy(e => e.CreatedTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventType)))
                .ToListAsync();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> GetFailedEventAsync()
        {
            var query = $"SELECT TOP(50) * FROM IntegrationEventLogEntry WITH(UPDLOCK, READPAST)" +
                        $"WHERE State = @State " +
                        $"ORDER BY CreatedTime ASC";
            var param = new SqlParameter("@State",
                $"{(int)IntegrationEventLogEntry.IntegrationEventLogState.PublishFailed}");

            return await _efIntegrationEventLogContext.IntegrationEventLogEntries
                .FromSql(query, param)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventType)))
                .ToListAsync();
        }

        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new AggregateException("Method must has created transaction before call");
            var eventLog = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            _efIntegrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _efIntegrationEventLogContext.IntegrationEventLogEntries.Add(eventLog);

            return _efIntegrationEventLogContext.SaveChangesAsync();
        }

        public Task SaveMultiEventAsync(List<IntegrationEvent> events, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new AggregateException("Method must has created transaction before call");
            if (events == null || !events.Any())
            {
                var ex = new Exception("No event to fire");
                Logger.LogError(ex, ex.Message);
                throw ex;
            }

            _efIntegrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());

            events.ForEach(@event =>
            {
                var eventLog = new IntegrationEventLogEntry(@event, transaction.TransactionId);
                _efIntegrationEventLogContext.IntegrationEventLogEntries.Add(eventLog);
            });

            return _efIntegrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventState(eventId, IntegrationEventLogEntry.IntegrationEventLogState.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventState(eventId, IntegrationEventLogEntry.IntegrationEventLogState.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventState(eventId, IntegrationEventLogEntry.IntegrationEventLogState.PublishFailed);
        }

        private async Task<int> UpdateEventState(Guid eventId, IntegrationEventLogEntry.IntegrationEventLogState state)
        {
            var eventLog = await _efIntegrationEventLogContext.IntegrationEventLogEntries.Where(e => e.EventId == eventId).Select(x => new { x.TimeSent, x.State }).SingleAsync();

            var timeSent = eventLog.TimeSent;

            if (state == IntegrationEventLogEntry.IntegrationEventLogState.InProgress)
            {
                timeSent++;
            }

            await _efIntegrationEventLogContext.Database.ExecuteSqlCommandAsync($"UPDATE IntegrationEventLogEntry SET [State] = {(byte)state}, [TimeSent] = {timeSent} WHERE EventId = {eventId}");

            return await _efIntegrationEventLogContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> GetPublishableEventByIdAsync(Guid eventId)
        {
            return await _efIntegrationEventLogContext.IntegrationEventLogEntries
                       .Where(e => e.EventId == eventId &&
                                   e.State == IntegrationEventLogEntry.IntegrationEventLogState.NotPublished)
                       .OrderBy(e => e.CreatedTime)
                       .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventType)))
                       .ToListAsync();

        }

        public async Task<IntegrationEventLogEntry> AddEventLogAsync(IntegrationEvent @event, Guid? transactionId)
        {            
            var logTransactionId = transactionId.GetValueOrDefault(Guid.NewGuid());
            var eventLog = new IntegrationEventLogEntry(@event, logTransactionId)
            {
                ProcessedTime = DateTime.Now
            };
            _efIntegrationEventLogContext.IntegrationEventLogEntries.Add(eventLog);
            await _efIntegrationEventLogContext.SaveChangesAsync();
            return eventLog;
        }

    }
}
