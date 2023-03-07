using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.UpdateClocking
{
    public class UpdateClockingTimeCommandHandler : BaseCommandHandler, IRequestHandler<UpdateClockingTimeCommand, ClockingDto>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly ILogger<UpdateClockingTimeCommandHandler> _logger;

        public UpdateClockingTimeCommandHandler(
            IEventDispatcher eventDispatcher,
            ILogger<UpdateClockingTimeCommandHandler> logger,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
        : base(eventDispatcher)
        {
            _logger = logger;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<ClockingDto> Handle(UpdateClockingTimeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _timeSheetIntegrationEventService.PublishEventsToEventBusAsync(new UpdateClockingTimeIntegrationEvent(request.TenantId, request.BranchId, request.ShiftId, request.ShiftName, request.From, request.To));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            return null;
        }
    }
}