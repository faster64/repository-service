using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.Events;
using KiotVietTimeSheet.DomainEventProcessWorker.Persistence;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.DomainEventProcessWorker.KvProcessFailEvent
{
    public class KvProcessFailEventService : IKvProcessFailEventService
    {
        private readonly KvMasterDataDbContext _db;
        private readonly List<Type> _evenTypes;

        private ILogger<KvProcessFailEventService> _logger;
        private ILogger<KvProcessFailEventService> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<KvProcessFailEventService>>());

        public KvProcessFailEventService(KvMasterDataDbContext db)
        {
            _db = db;
            _evenTypes = Assembly.GetAssembly(typeof(RabbitPublishFailIntegrationEvent))
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task MarkIsInprogressAsync(long id)
        {
            await UpdateStateAsync(id, KvProcessFailEventStates.InProgress);
        }

        public async Task MarkIsResolvedAsync(long id)
        {
            await UpdateStateAsync(id, KvProcessFailEventStates.Resolved);
        }

        public async Task MarkIsPendingAsync(long id, string msg = "")
        {
            await UpdateStateAsync(id, KvProcessFailEventStates.Pending, msg);
        }

        public async Task RetryKvProcessFailEvents(Func<IntegrationEvent, Task> retryFunc)
        {
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var kvProcessFailEvents = await _db.KvProcessFailEvent
                        .FromSql($"SELECT TOP(100) " +
                                 $"* " +
                                 $"FROM KvProcessFailEvent WITH(UPDLOCK, READPAST) " +
                                 $"WHERE State = {(byte)KvProcessFailEventStates.Pending} " +
                                 $"ORDER BY CreatedTime ASC")
                        .Select(e => e.DeserializeJsonContent(_evenTypes.Find(t => t.Name == e.EventType)))
                        .ToListAsync();

                    foreach (var kvProcessFailEvent in kvProcessFailEvents)
                    {
                        try
                        {
                            await MarkIsInprogressAsync(kvProcessFailEvent.Id);
                            await retryFunc(kvProcessFailEvent.IntegrationEvent);
                            await MarkIsResolvedAsync(kvProcessFailEvent.Id);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, ex.Message);
                            await MarkIsPendingAsync(kvProcessFailEvent.Id, ex.Message);
                        }

                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task UpdateStateAsync(long id, KvProcessFailEventStates state, string msg = "")
        {
            var kvProcessFailEvent =
                await _db.KvProcessFailEvent.SingleAsync(eventLog => eventLog.Id == id);

            if (kvProcessFailEvent == null) return;

            kvProcessFailEvent.State = (byte)state;

            if (kvProcessFailEvent.State == (byte)KvProcessFailEventStates.InProgress)
            {
                kvProcessFailEvent.RetryTimes++;
                kvProcessFailEvent.ProcessedTime = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(msg) && kvProcessFailEvent.State == (byte)KvProcessFailEventStates.Pending)
            {
                kvProcessFailEvent.UpdateErrorMessage(msg);
            }

            _db.KvProcessFailEvent.Update(kvProcessFailEvent);
            await _db.SaveChangesAsync();
        }
    }
}
