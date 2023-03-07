using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePenalize
{
    public class UpdatePenalizeCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdatePenalizeCommand, PenalizeDto>
    {
        private readonly IMapper _mapper;
        private readonly IPenalizeWriteOnlyRepository _penalizeWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly PenalizeCreateOrUpdateValidator _penalizeCreateOrUpdateValidator;
        public UpdatePenalizeCommandHandler(
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
        public async Task<PenalizeDto> Handle(UpdatePenalizeCommand request, CancellationToken cancellationToken)
        {
            var existingPenalize = await _penalizeWriteOnlyRepository.FindByIdAsync(request.Penalize.Id);
            if (existingPenalize == null)
            {
                NotifyValidationErrors(typeof(Penalize), new List<string> { string.Format(Message.not_exists, Label.penalize) });
                return null;
            }
            
            var validator = new ValidationResult();
            if (!existingPenalize.Name.ToLower().Equals(request.Penalize.Name.ToLower()))
            {
                var penalize = new Penalize(request.Penalize.Name);
                validator = await _penalizeCreateOrUpdateValidator.ValidateAsync(penalize);
            }

            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Penalize), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return request.Penalize;
            }

            var penalizeOld = existingPenalize.CreateCopy(existingPenalize);
            existingPenalize.Update(request.Penalize.Name);

            _penalizeWriteOnlyRepository.Update(existingPenalize);
            await _penalizeWriteOnlyRepository.UnitOfWork.CommitAsync();
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedPenalizeIntegrationEvent(existingPenalize, penalizeOld));
            return _mapper.Map<PenalizeDto>(existingPenalize);
        }
    }
}
