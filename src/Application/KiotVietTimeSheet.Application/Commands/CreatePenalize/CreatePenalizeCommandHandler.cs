using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreatePenalize
{
    public class CreatePenalizeCommandHandler : BaseCommandHandler,
        IRequestHandler<CreatePenalizeCommand, PenalizeDto>
    {
        private readonly IMapper _mapper;
        private readonly IPenalizeWriteOnlyRepository _penalizeWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly PenalizeCreateOrUpdateValidator _penalizeCreateOrUpdateValidator;
        public CreatePenalizeCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPenalizeWriteOnlyRepository penalizeWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            PenalizeCreateOrUpdateValidator penalizeCreateOrUpdateValidator
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _penalizeWriteOnlyRepository = penalizeWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _penalizeCreateOrUpdateValidator = penalizeCreateOrUpdateValidator;
        }
        public async Task<PenalizeDto> Handle(CreatePenalizeCommand request, CancellationToken cancellationToken)
        {
            var penalize = new Penalize(request.Penalize.Name);
            var validator = await _penalizeCreateOrUpdateValidator.ValidateAsync(penalize);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Penalize), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return request.Penalize;
            }
            
            _penalizeWriteOnlyRepository.Add(penalize);
            await _penalizeWriteOnlyRepository.UnitOfWork.CommitAsync();
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedPenalizeIntegrationEvent(penalize));
            return _mapper.Map<PenalizeDto>(penalize);
        }
    }
}
