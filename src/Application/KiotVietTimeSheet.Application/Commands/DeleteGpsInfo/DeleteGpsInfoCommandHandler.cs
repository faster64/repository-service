using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.DeleteGpsInfo
{
    public class DeleteGpsInfoCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteGpsInfoCommand, Unit>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IGpsInfoWriteOnlyRepository _gpsInfoWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public DeleteGpsInfoCommandHandler(
            IEventDispatcher eventDispatcher,
            IGpsInfoWriteOnlyRepository gpsInfoWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService) 
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _gpsInfoWriteOnlyRepository = gpsInfoWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }
        public async Task<Unit> Handle(DeleteGpsInfoCommand request, CancellationToken cancellationToken)
        {
            var gpsInfo = await _gpsInfoWriteOnlyRepository.FindByIdAsync(request.Id);
            if (gpsInfo == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(GpsInfo).Name, string.Format(Message.not_exists)));
                return Unit.Value;
            }
            // Audit log
            gpsInfo.Delete();
            _gpsInfoWriteOnlyRepository.Delete(gpsInfo);
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedGpsInfoIntegrationEvent(gpsInfo));
            await _gpsInfoWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
