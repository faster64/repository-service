using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Commands.CreateDeduction
{
    public class CreateDeductionCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateDeductionCommand, DeductionDto>
    {
        private readonly IMapper _mapper;
        private readonly DeductionCreateOrUpdateValidator _deductionCreateOrUpdateValidator;
        private readonly IDeductionWriteOnlyRepository _deductionWriteOnlyRepository;

        public CreateDeductionCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            DeductionCreateOrUpdateValidator deductionCreateOrUpdateValidator,
            IDeductionWriteOnlyRepository iDeductionWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _deductionCreateOrUpdateValidator = deductionCreateOrUpdateValidator;
            _deductionWriteOnlyRepository = iDeductionWriteOnlyRepository;
        }

        public async Task<DeductionDto> Handle(CreateDeductionCommand request, CancellationToken cancellationToken)
        {
            var deductionDto = request.Deduction;
            var deduction = new Deduction(deductionDto.Name, deductionDto.ValueType.GetValueOrDefault());
            var validator = await _deductionCreateOrUpdateValidator.ValidateAsync(deduction);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Deduction), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return deductionDto;
            }
            deduction.SetDeductionRuleValueDetail(request.DeductionDetail);
            _deductionWriteOnlyRepository.Add(deduction);
            await _deductionWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<DeductionDto>(deduction);
        }
    }
}