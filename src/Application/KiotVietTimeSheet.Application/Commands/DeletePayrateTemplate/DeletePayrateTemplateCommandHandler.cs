using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeletePayrateTemplate
{
    public class DeletePayrateTemplateCommandHandler : BaseCommandHandler,
        IRequestHandler<DeletePayrateTemplateCommand, Unit>
    {
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        private readonly IPayRateReadOnlyRepository _payRateReadOnlyRepository;
        private readonly IPayRateWriteOnlyRepository _payRateWriteOnlyRepository;
        private readonly IPayRateTemplateWriteOnlyRepository _payRateTemplateWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IEventDispatcher _eventDispatcher;

        public DeletePayrateTemplateCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IPayRateReadOnlyRepository payRateReadOnlyRepository,
            IPayRateWriteOnlyRepository payRateWriteOnlyRepository,
            IPayRateTemplateWriteOnlyRepository payRateTemplateWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            _payRateReadOnlyRepository = payRateReadOnlyRepository;
            _payRateWriteOnlyRepository = payRateWriteOnlyRepository;
            _payRateTemplateWriteOnlyRepository = payRateTemplateWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<Unit> Handle(DeletePayrateTemplateCommand request, CancellationToken cancellationToken)
        {
            var existPayRateTemplate = await _payRateTemplateReadOnlyRepository.FindByIdAsync(request.Id);

            if (existPayRateTemplate == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(PayRateTemplate).Name, @"Mẫu lương không tồn tại"));
                return Unit.Value;
            }

            var deleteValidator = await new PayRateTemplateDeteleValidator().ValidateAsync(existPayRateTemplate);
            if (!deleteValidator.IsValid)
            {
                NotifyValidationErrors(typeof(PayRateTemplate), deleteValidator.Errors);
                return Unit.Value;
            }

            existPayRateTemplate.Status = (int)PayRateTemplateStatus.Deleted; 
            _payRateTemplateWriteOnlyRepository.Delete(existPayRateTemplate);

            // Audit trail
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedPayRateTemplateIntegrationEvent(existPayRateTemplate, request.IsGeneralSetting));

            await _payRateTemplateWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }

    }
}
