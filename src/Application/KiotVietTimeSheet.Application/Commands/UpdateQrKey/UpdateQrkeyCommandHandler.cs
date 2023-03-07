using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.UpdateQrKey
{
    public class UpdateQrkeyCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateQrkeyCommand, string>
    {
        private readonly IGpsInfoWriteOnlyRepository _gpsInfoWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;

        public UpdateQrkeyCommandHandler(IEventDispatcher eventDispatcher,
            IGpsInfoWriteOnlyRepository gpsInfoWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            IKiotVietServiceClient kiotVietServiceClient) : base(eventDispatcher)
        {
            _gpsInfoWriteOnlyRepository = gpsInfoWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _kiotVietServiceClient = kiotVietServiceClient;
        }

        public async Task<string> Handle(UpdateQrkeyCommand request, CancellationToken cancellationToken)
        {
            async Task UpdateQrKeyAudit(GpsInfo gpsInfor)
            {
                var branch = gpsInfor != null ? await _kiotVietServiceClient.GetBranchById(gpsInfor.BranchId) : null;
                await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedQrKeyIntegrationEvent(gpsInfor, branch?.Name));
            }
            return await _gpsInfoWriteOnlyRepository.ChangeQrKey(request.GpsInfoId, UpdateQrKeyAudit);
        }
    }
}