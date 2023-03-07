using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using MediatR;
using AutoMapper;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.PayRateValidators;
using KiotVietTimeSheet.SharedKernel.Domain;
using System.Threading.Tasks;
using System.Threading;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using System.Linq;

namespace KiotVietTimeSheet.Application.Commands.UpdateAllowance
{
    public class UpdateAllowanceCommandHandler : BaseCommandHandler,
        IRequestHandler<UpdateAllowanceCommand, AllowanceDto>
    {
        private readonly IMapper _mapper;
        private readonly AllowanceCreateOrUpdateValidator _allowanceCreateOrUpdateValidator;
        private readonly IAllowanceWriteOnlyRepository _allowanceWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;

        public UpdateAllowanceCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IAllowanceWriteOnlyRepository allowanceWriteOnlyRepository,
            AllowanceCreateOrUpdateValidator allowanceCreateOrUpdateValidator,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository
            ) : base(eventDispatcher)
        {
            _mapper = mapper;
            _allowanceCreateOrUpdateValidator = allowanceCreateOrUpdateValidator;
            _allowanceWriteOnlyRepository = allowanceWriteOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
        }

        public async Task<AllowanceDto> Handle(UpdateAllowanceCommand request, CancellationToken cancellationToken)
        {
            var allowanceDto = request.Allowance;
            var existingAllowanceById = await _allowanceReadOnlyRepository.FindByIdAsync(allowanceDto.Id);
            if (existingAllowanceById == null)
            {
                NotifyValidationErrors(typeof(Allowance), new List<string> { "Phụ cấp không tồn tại" });
                return null;
            }

            existingAllowanceById.Update(allowanceDto.Name, allowanceDto.Type, allowanceDto.IsChecked, allowanceDto.ValueRatio, allowanceDto.Value, allowanceDto.Rank);

            var validator = await _allowanceCreateOrUpdateValidator.ValidateAsync(existingAllowanceById);
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Allowance), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return null;
            }

            _allowanceWriteOnlyRepository.Update(existingAllowanceById);
            await _allowanceWriteOnlyRepository.UnitOfWork.CommitAsync();
            return _mapper.Map<AllowanceDto>(existingAllowanceById);
        }
    }
}
