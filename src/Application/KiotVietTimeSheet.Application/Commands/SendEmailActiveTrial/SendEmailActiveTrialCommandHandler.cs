using KiotVietTimeSheet.Application.Abstractions;
using MediatR;
using KiotVietTimeSheet.SharedKernel.Domain;
using System.Threading.Tasks;
using System.Threading;
using KiotVietTimeSheet.Application.EventBus.Events.SendMailEvents;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Application.Commands.SendEmailActiveTrial
{
    public class SendEmailActiveTrialCommandHandler : BaseCommandHandler,
        IRequestHandler<SendEmailActiveTrialCommand, Unit>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public SendEmailActiveTrialCommandHandler(
            IEventDispatcher eventDispatcher,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
            ) : base(eventDispatcher)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<Unit> Handle(SendEmailActiveTrialCommand request, CancellationToken cancellationToken)
        {
            var emailEvent = new EmailEvent
            {
                Subject = request.Subject,
                Body = request.Body,
            };
            await _timeSheetIntegrationEventService.AddEventAsync(new SentMailIntegrationEvent(emailEvent));
            return new Unit();
        }
    }
}
