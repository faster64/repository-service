using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdateDeduction
{
    public class UpdateDeductionCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateDeductionCommand, DeductionDto>
    {
        private readonly IMapper _mapper;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly DeductionCreateOrUpdateValidator _deductionCreateOrUpdateValidator;
        private readonly IDeductionWriteOnlyRepository _deductionWriteOnlyRepository;

        public UpdateDeductionCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IDeductionReadOnlyRepository iDeductionReadOnlyRepository,
            DeductionCreateOrUpdateValidator deductionCreateOrUpdateValidator,
            IDeductionWriteOnlyRepository iDeductionWriteOnlyRepository
        ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _deductionReadOnlyRepository = iDeductionReadOnlyRepository;
            _deductionCreateOrUpdateValidator = deductionCreateOrUpdateValidator;
            _deductionWriteOnlyRepository = iDeductionWriteOnlyRepository;
        }
        public async Task<DeductionDto> Handle(UpdateDeductionCommand request, CancellationToken cancellationToken)
        {
            var deductionDto = request.Deduction;
            var existingDeduction = await _deductionReadOnlyRepository.FindByIdAsync(deductionDto.Id);
            if (existingDeduction == null)
            {
                NotifyValidationErrors(typeof(Deduction), new List<string> { "Giảm trừ không tồn tại" });
                return null;
            }
            existingDeduction.Update(deductionDto.Name, deductionDto.ValueType.GetValueOrDefault(), request.DeductionDetail);

            var validator = await _deductionCreateOrUpdateValidator.ValidateAsync(existingDeduction, cancellationToken);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Deduction), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }
            _deductionWriteOnlyRepository.Update(existingDeduction);
            await _deductionWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<DeductionDto>(existingDeduction);
        }
    }
}
