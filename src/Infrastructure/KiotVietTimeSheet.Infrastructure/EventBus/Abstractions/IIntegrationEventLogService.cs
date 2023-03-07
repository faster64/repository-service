using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Infrastructure.EventBus.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore.Storage;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> GetPublishableEventByIdAsync(Guid eventId);

        Task<IEnumerable<IntegrationEventLogEntry>> GetPublishableEventAsync(Guid transactionId);

        Task<IEnumerable<IntegrationEventLogEntry>> GetFailedEventAsync();

        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task<IntegrationEventLogEntry> AddEventLogAsync(IntegrationEvent @event, Guid? transactionId);

        Task SaveMultiEventAsync(List<IntegrationEvent> events, IDbContextTransaction transaction);

        Task MarkEventAsPublishedAsync(Guid eventId);

        Task MarkEventAsInProgressAsync(Guid eventId);

        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
