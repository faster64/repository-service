using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CopyPayrateTemplate
{
    public class CopyPayrateTemplateCommandHandler : BaseCommandHandler,
        IRequestHandler<CopyPayrateTemplateCommand, PayRateFormDto>
    {
        private readonly IMapper _mapper;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        private readonly IPayRateTemplateWriteOnlyRepository _payRateTemplateWriteOnlyRepository;

        public CopyPayrateTemplateCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IPayRateTemplateWriteOnlyRepository payRateTemplateWriteOnlyRepository
        )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _mapper = mapper;
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            _payRateTemplateWriteOnlyRepository = payRateTemplateWriteOnlyRepository;
        }

        public async Task<PayRateFormDto> Handle(CopyPayrateTemplateCommand request, CancellationToken cancellationToken)
        {
            var existTemplate = await _payRateTemplateReadOnlyRepository.FindByIdAsync(request.Id, true);
            if (existTemplate == null)
            {
                NotifyPayRateTemplateInDbIsNotExists();
                return null;
            }

            var copyTemplate = PayRateTemplate.Instance;
            copyTemplate.Copy(request.Name, existTemplate);

            var copyTemplateValidator = await new PayRateTemplateCopyValidator().ValidateAsync(copyTemplate);
            if (!copyTemplateValidator.IsValid)
            {
                NotifyValidationErrors(typeof(PayRateTemplate), copyTemplateValidator.Errors);
                return null;
            }

            _payRateTemplateWriteOnlyRepository.Add(copyTemplate);
            await _payRateTemplateWriteOnlyRepository.UnitOfWork.CommitAsync();

            return _mapper.Map<PayRateFormDto>(copyTemplate);
        }

        private void NotifyPayRateTemplateInDbIsNotExists()
        {
            _eventDispatcher.FireEvent(new DomainNotification(typeof(PayRateTemplate).Name, @"Mẫu lương không tồn tại"));
        }

    }
}
