using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Infrastructure.Persistence.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.AuditTrail
{
    public class AuditProcessFailEventService : IAuditProcessFailEventService
    {
        private readonly EfDbContext _db;
        private readonly List<Type> _evenTypes;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private ILogger<AuditProcessFailEventService> _logger;
        private ILogger<AuditProcessFailEventService> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<AuditProcessFailEventService>>());

        public AuditProcessFailEventService(EfDbContext db)
        {
            _db = db;
            _evenTypes = Assembly.GetAssembly(typeof(CreatedShiftIntegrationEvent))
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task AddAsync(AuditProcessFailEvent @event)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (await _db.AuditProcessFailEvents.AnyAsync(e => e.EventId == @event.EventId))
                {
                    var ex = new Exception(@event.ErrorMessage);
                    Logger.LogError(ex, ex.Message);
                    throw ex;
                }

                _db.Add(@event);
                await _db.SaveChangesAsync();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task MarkIsInprogressAsync(long id)
        {
            await UpdateStateAsync(id, AuditProcessFailEventStates.InProgress);
        }

        public async Task MarkIsResolvedAsync(long id)
        {
            await UpdateStateAsync(id, AuditProcessFailEventStates.Resolved);
        }

        public async Task MarkIsPendingAsync(long id, string msg = "")
        {
            await UpdateStateAsync(id, AuditProcessFailEventStates.Pending, msg);
        }

        public async Task RetryAuditProcessFailEvents(Func<IntegrationEvent, Task> retryFunc)
        {
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var query = $"SELECT TOP(100) * FROM AuditProcessFailEvent WITH(UPDLOCK, READPAST)" +
                                $"WHERE State = @State " +
                                $"ORDER BY CreatedTime ASC";
                    var param = new SqlParameter("@State", $"{(int)AuditProcessFailEventStates.Pending}");

                    var auditProcessFailEvents = await _db.AuditProcessFailEvents
                        .FromSql(query, param)
                        .Select(e => e.DeserializeJsonContent(_evenTypes.Find(t => t.Name == e.EventType)))
                        .ToListAsync();

                    foreach (var auditProcessFailEvent in auditProcessFailEvents)
                    {
                        try
                        {
                            await MarkIsInprogressAsync(auditProcessFailEvent.Id);
                            await retryFunc(auditProcessFailEvent.IntegrationEvent);
                            await MarkIsResolvedAsync(auditProcessFailEvent.Id);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, ex.Message);
                            await MarkIsPendingAsync(auditProcessFailEvent.Id, ex.Message);
                        }

                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                    trans.Rollback();
                }
            }
        }

        private async Task UpdateStateAsync(long id, AuditProcessFailEventStates state, string msg = "")
        {
            var auditProcessFailEvent =
                await _db.AuditProcessFailEvents.SingleAsync(eventLog => eventLog.Id == id);

            if (auditProcessFailEvent == null) return;

            auditProcessFailEvent.State = state;

            if (auditProcessFailEvent.State == AuditProcessFailEventStates.InProgress)
            {
                auditProcessFailEvent.RetryTimes++;
                auditProcessFailEvent.ProcessedTime = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(msg) && auditProcessFailEvent.State == AuditProcessFailEventStates.Pending)
            {
                auditProcessFailEvent.UpdateErrorMessage(msg);
            }

            _db.AuditProcessFailEvents.Update(auditProcessFailEvent);
            await _db.SaveChangesAsync();
        }
    }
}
